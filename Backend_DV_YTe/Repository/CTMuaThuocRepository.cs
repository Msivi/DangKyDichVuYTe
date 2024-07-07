using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Repository
{
    public class CTMuaThuocRepository:ICTMuaThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTMuaThuocRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        public async Task<string> CreateCTMuaThuoc(CTMuaThuocEntity ctMua)
        {
            var existingCTMuaThuoc = await _context.cTMuaThuocEntities
                .FirstOrDefaultAsync(c => c.MaHoaDon == ctMua.MaHoaDon && c.MaThuoc == ctMua.MaThuoc && (c.DeletedTime == null ));
            var existingThuoc = await _context.thuocEntities.FirstOrDefaultAsync(c => c.Id == ctMua.MaThuoc && c.DeletedTime == null);
            var existingHoaDon = await _context.hoaDonEntities.FirstOrDefaultAsync(c => c.Id == ctMua.MaHoaDon && c.DeletedTime == null);

            if (existingCTMuaThuoc != null)// dã có thuốc r h cập nhật thêm vào
            {

                var slThuocTrongLo = await _context.loThuocEntities
                            .Where(l => l.MaThuoc == ctMua.MaThuoc && l.DeletedTime == null)
                            .SumAsync(l => l.soLuong);
                if (ctMua.soLuong > slThuocTrongLo)
                {
                    throw new Exception("Không đủ số lượng trong kho. Sô lượng hiện tại có thể mua là: " + slThuocTrongLo);
                }
                var loThuoc = await _context.loThuocEntities
                                     .Where(l => l.MaThuoc == ctMua.MaThuoc && l.DeletedTime == null)
                                     .OrderByDescending(l => l.CreateTimes)
                                     .FirstOrDefaultAsync();

                existingCTMuaThuoc.soLuong += ctMua.soLuong;

                var giaLech = ctMua.soLuong * existingThuoc.donGia;

                existingCTMuaThuoc.thanhTien += giaLech;
                _context.cTMuaThuocEntities.Update(existingCTMuaThuoc); // nếu khách đã chọn trước r thì cập nhật thêm số lượng vào
                
                existingHoaDon.tongTien += giaLech;
                _context.hoaDonEntities.Update(existingHoaDon);


            }
            else // chưa có sp này
            {
                var existingLoThuoc = await _context.loThuocEntities
                                         .Where(l => l.MaThuoc == ctMua.MaThuoc && l.DeletedTime == null)
                                         .OrderByDescending(l => l.CreateTimes)
                                         .FirstOrDefaultAsync();
                if (existingLoThuoc == null)
                {
                    throw new Exception(message: "Mã thuốc không tồn tại!");
                }

                var existingMaHoaDon = await _context.hoaDonEntities
                    .FirstOrDefaultAsync(c => c.Id == ctMua.MaHoaDon && c.DeletedTime == null);
                if (existingMaHoaDon == null)
                {
                    throw new Exception(message: "Mã hóa đơn không tồn tại!");
                }
                var slThuocTrongLo = await _context.loThuocEntities
                            .Where(l => l.MaThuoc == ctMua.MaThuoc && l.DeletedTime == null)
                            .SumAsync(l => l.soLuong);
                if (slThuocTrongLo < ctMua.soLuong)
                {
                    throw new Exception(message: $"Số lượng thuốc trong kho không đủ! Số lượng trong kho là:" + slThuocTrongLo);
                }
               

                ctMua.donGia = existingThuoc.donGia;
                ctMua.thanhTien = ctMua.soLuong * existingThuoc.donGia;

                await _context.cTMuaThuocEntities.AddAsync(ctMua);

                existingHoaDon.tongTien += ctMua.thanhTien;
                _context.hoaDonEntities.Update(existingHoaDon);
            }

            await _context.SaveChangesAsync();

            return $"{ctMua.MaHoaDon}-{ctMua.MaThuoc}";
        }

        public async Task<CTMuaThuocEntity> DeleteCTMuaThuoc(int maHD, int maThuoc, bool isPhysical)
        {
            try
            {
                var entity = await _context.cTMuaThuocEntities.FirstOrDefaultAsync(c => c.MaHoaDon == maHD&& c.MaThuoc== maThuoc&& c.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy Id chi tiết nhập!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.cTMuaThuocEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.cTMuaThuocEntities.Update(entity);
                    }

                    await _context.SaveChangesAsync();

                }
                return entity;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<CTMuaThuocEntity>> GetAllCTMuaThuoc()
        {
            try
            {
                var entities = await _context.cTMuaThuocEntities
                     .Where(c => c.DeletedTime == null)
                     .ToListAsync();

                if (entities is null)
                {
                    throw new Exception("Empty list!");
                }
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ICollection<CTMuaThuocEntity>> GetAllCTMuaThuocKH()
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId"); // Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception("Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var hoaDonIds = await _context.hoaDonEntities
                    .Where(h => h.MaKhachHang == userId)
                    .Select(h => h.Id)
                    .ToListAsync();

                var ctmuaThuocEntities = await _context.cTMuaThuocEntities
                                              .Where(ct => hoaDonIds.Contains(ct.MaHoaDon))
                                              .ToListAsync();

                if (ctmuaThuocEntities == null || ctmuaThuocEntities.Count == 0)
                {
                    throw new Exception("Danh sách trống!");
                }
                return ctmuaThuocEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CTMuaThuocEntity> GetCTMuaThuocById(int maHD, int maThuoc)
        {
            var entity = await _context.cTMuaThuocEntities.FirstOrDefaultAsync(c => c.MaHoaDon == maHD && c.MaThuoc==maThuoc&& c.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }


        public async Task UpdateCTMuaThuoc(int maHD, int maThuoc, CTMuaThuocModel entity)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var existingCTMuaThuoc = await _context.cTMuaThuocEntities
                .FirstOrDefaultAsync(c => c.MaHoaDon == maHD && c.MaThuoc == maThuoc && c.DeletedTime == null);
            var slThuocTrongLo = await _context.loThuocEntities
                           .Where(l => l.MaThuoc == maThuoc && l.DeletedTime == null)
                           .SumAsync(l => l.soLuong);

            if (existingCTMuaThuoc == null)
            {
                throw new Exception(message: "Không tìm thấy thông tin mua thuốc!");
            }

            var existingMaThuoc = await _context.thuocEntities
               .FirstOrDefaultAsync(c => c.Id == entity.MaThuoc && c.DeletedTime == null);
            if (existingMaThuoc == null)
            {
                throw new Exception(message: "Mã thuốc không tồn tại!");
            }

            var existingMaHoaDon = await _context.hoaDonEntities
                .FirstOrDefaultAsync(c => c.Id == entity.MaHoaDon && c.DeletedTime == null);
            if (existingMaHoaDon == null)
            {
                throw new Exception(message: "Mã hóa đơn không tồn tại!");
            }

            if (existingCTMuaThuoc.soLuong > entity.soLuong)// số lượng mua lại bé hơn
            {
                //existingMaThuoc.soLuong += existingCTMuaThuoc.soLuong - entity.soLuong;// cộng lại số lượng dư vào thuốc

                var giaLech = (existingCTMuaThuoc.soLuong - entity.soLuong) * existingMaThuoc.donGia;

                existingCTMuaThuoc.soLuong = entity.soLuong; // so luong cập nhật
                existingCTMuaThuoc.donGia = existingMaThuoc.donGia;
                existingCTMuaThuoc.thanhTien -= giaLech;
                existingCTMuaThuoc.CreateBy = userId;

                //_context.thuocEntities.Update(existingMaThuoc);
                // cập nhật lại cái hóa đơn
                existingMaHoaDon.tongTien -= giaLech;
                _context.hoaDonEntities.Update(existingMaHoaDon);

            }
            else // số lượng mua lớn hơn
            {
                if (slThuocTrongLo < (entity.soLuong - existingCTMuaThuoc.soLuong))
                {
                    throw new Exception(message: $"Số lượng hiện tại trong kho không đủ để thực hiện cập nhật này! Số lượng hiện tại kho đang là: {slThuocTrongLo}");
                }
                else
                {
                    //existingMaThuoc.soLuong -= entity.soLuong - existingCTMuaThuoc.soLuong; // cập nhật số lượng thuốc trong kho
                    var giaLech = (entity.soLuong - existingCTMuaThuoc.soLuong) * existingMaThuoc.donGia;
                    existingCTMuaThuoc.soLuong = entity.soLuong;
                    existingCTMuaThuoc.donGia = existingMaThuoc.donGia;
                    existingCTMuaThuoc.thanhTien += giaLech;
                    existingCTMuaThuoc.CreateBy = userId;

                    //_context.thuocEntities.Update( existingMaThuoc);
                    // caap nhat laij cai hoa don
                    existingMaHoaDon.tongTien += giaLech;
                    _context.hoaDonEntities.Update(existingMaHoaDon);


                }

            }

            //_context.Entry(existingCTMuaThuoc).CurrentValues.SetValues(entity);
            _context.cTMuaThuocEntities.Update(existingCTMuaThuoc);
            await _context.SaveChangesAsync();
        }
    }
}
