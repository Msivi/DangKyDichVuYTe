using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Repository
{
    public class CTMuaThietBiYTeRepository:ICTMuaThietBiYTeRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTMuaThietBiYTeRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        public async Task<string> CreateCTMuaThietBiYTe(CTMuaThietBiYTeEntity ctMua)
        {
            var existingCTMuaThietBiYTe = await _context.cTMuaThietBiYTeEntities
                .FirstOrDefaultAsync(c => c.MaHoaDon == ctMua.MaHoaDon && c.MaThietBiYTe == ctMua.MaThietBiYTe && c.DeletedTime == null);

            var existingHoaDon = await _context.hoaDonEntities.FirstOrDefaultAsync(c => c.Id == ctMua.MaHoaDon && c.DeletedTime == null);

            if (existingCTMuaThietBiYTe != null) // có mặc hàng r
            {
                var existingThietBi = await _context.thietBiYTeEntities
                   .FirstOrDefaultAsync(c => c.Id == ctMua.MaThietBiYTe && c.DeletedTime == null);
                if (ctMua.soLuong > existingThietBi.soLuong)
                {
                    throw new Exception("Không đủ số lượng trong kho. Sô lượng hiện tại có thể mua là: " + existingThietBi.soLuong);
                }

                existingCTMuaThietBiYTe.soLuong += ctMua.soLuong;

                var khoangChenhLech =   (ctMua.soLuong * existingThietBi.donGia);

                existingCTMuaThietBiYTe.thanhTien += khoangChenhLech;
                _context.cTMuaThietBiYTeEntities.Update(existingCTMuaThietBiYTe); // nếu khách đã chọn trước r thì cập nhật thêm số lượng vào
                // cập nhật lại sl thuốc trong kho
                existingThietBi.soLuong -= ctMua.soLuong;
                _context.thietBiYTeEntities.Update(existingThietBi); 
                // cập nhật lại giá tiền trong hóa đơn
                existingHoaDon.tongTien += khoangChenhLech; 
                _context.hoaDonEntities.Update(existingHoaDon);

            }
            else // chưa có mặt hàng
            {
                var existingThietBi = await _context.thietBiYTeEntities
                    .FirstOrDefaultAsync(c => c.Id == ctMua.MaThietBiYTe && c.DeletedTime == null);
                if (existingThietBi == null)
                {
                    throw new Exception(message: "Mã thiết bị không tồn tại!");
                }

                var existingMaHoaDon = await _context.hoaDonEntities
                    .FirstOrDefaultAsync(c => c.Id == ctMua.MaHoaDon && c.DeletedTime == null);
                if (existingMaHoaDon == null)
                {
                    throw new Exception(message: "Mã hóa đơn không tồn tại!");
                }
                if (existingThietBi.soLuong < ctMua.soLuong)
                {
                    throw new Exception(message: $"Số lượng thiết bị trong kho không đủ! Số lượng trong kho là:" + existingThietBi.soLuong);
                }
                existingThietBi.soLuong -= ctMua.soLuong;
                _context.thietBiYTeEntities.Update(existingThietBi);

                ctMua.donGia = existingThietBi.donGia;
                ctMua.thanhTien = ctMua.soLuong * existingThietBi.donGia;

                await _context.cTMuaThietBiYTeEntities.AddAsync(ctMua);

                // cập nhật lại tổng tiền bên hóa đơn
                existingHoaDon.tongTien += ctMua.thanhTien;
                _context.hoaDonEntities.Update(existingHoaDon);
            }

            await _context.SaveChangesAsync();

            return $"{ctMua.MaHoaDon}-{ctMua.MaThietBiYTe}";
        }

        public async Task<CTMuaThietBiYTeEntity> DeleteCTMuaThietBiYTe(int maHD, int maTB, bool isPhysical)
        {
            try
            {
                var entity = await _context.cTMuaThietBiYTeEntities.FirstOrDefaultAsync(c => c.MaHoaDon == maHD && c.MaThietBiYTe == maTB && c.DeletedTime == null);
                 
                var existingHoaDon= await _context.hoaDonEntities.FirstOrDefaultAsync(c=>c.Id==maHD && c.DeletedTime==null);

                existingHoaDon.tongTien -= entity.thanhTien;
                _context.hoaDonEntities.Update(existingHoaDon);

                if (entity == null)
                {
                    throw new Exception("Không tìm thấy Id chi tiết nhập!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.cTMuaThietBiYTeEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.cTMuaThietBiYTeEntities.Update(entity);
                    }

                }

                await _context.SaveChangesAsync();
                return entity;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<CTMuaThietBiYTeEntity>> GetAllCTMuaThietBiYTe()
        {
            try
            {
                var entities = await _context.cTMuaThietBiYTeEntities
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

        public async Task<CTMuaThietBiYTeEntity> GetCTMuaThietBiYTeById(int maHD, int maTB)
        {
            var entity = await _context.cTMuaThietBiYTeEntities.FirstOrDefaultAsync(c => c.MaHoaDon == maHD && c.MaThietBiYTe == maTB && c.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }


        public async Task UpdateCTMuaThietBiYTe(int maHD, int maTB, CTMuaThietBiYTeModel entity)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var existingCTMuaThietBiYTe = await _context.cTMuaThietBiYTeEntities
                .FirstOrDefaultAsync(c => c.MaHoaDon == maHD && c.MaThietBiYTe == maTB && c.DeletedTime == null);

            if (existingCTMuaThietBiYTe == null)
            {
                throw new Exception(message: "Không tìm thấy thông tin mua thuốc!");
            }

            var existingThietBi = await _context.thietBiYTeEntities
               .FirstOrDefaultAsync(c => c.Id == entity.MaThietBiYTe && c.DeletedTime == null);
            if (existingThietBi == null)
            {
                throw new Exception(message: "Mã thuốc không tồn tại!");
            }

            var existingMaHoaDon = await _context.hoaDonEntities
                .FirstOrDefaultAsync(c => c.Id == entity.MaHoaDon && c.DeletedTime == null);
            if (existingMaHoaDon == null)
            {
                throw new Exception(message: "Mã hóa đơn không tồn tại!");
            }

            if (existingCTMuaThietBiYTe.soLuong > entity.soLuong)// số lượng mua lại, bé hơn
            {
                existingThietBi.soLuong += existingCTMuaThietBiYTe.soLuong - entity.soLuong; // cộng lại số lượng dư vào thiet bi

                var GiaLech = (existingCTMuaThietBiYTe.soLuong- entity.soLuong) * existingThietBi.donGia;

                existingCTMuaThietBiYTe.soLuong = entity.soLuong; // so luong cập nhật
                existingCTMuaThietBiYTe.donGia = existingThietBi.donGia;
                existingCTMuaThietBiYTe.thanhTien -=GiaLech ;
                existingCTMuaThietBiYTe.CreateBy = userId;

                existingMaHoaDon.tongTien -= GiaLech;
                _context.hoaDonEntities.Update(existingMaHoaDon);

                _context.thietBiYTeEntities.Update(existingThietBi);
            }
            else // số lượng mua lai, lớn hơn
            {
                if (existingThietBi.soLuong < (entity.soLuong - existingCTMuaThietBiYTe.soLuong))
                {
                    throw new Exception(message: $"Số lượng hiện tại trong kho không đủ để thực hiện cập nhật này! Số lượng hiện tại kho đang là: {existingThietBi.soLuong}");
                }
                else
                {
                    existingThietBi.soLuong -= entity.soLuong - existingCTMuaThietBiYTe.soLuong;

                    var giaLech = (entity.soLuong - existingCTMuaThietBiYTe.soLuong)  * existingThietBi.donGia;

                    existingCTMuaThietBiYTe.soLuong = entity.soLuong;
                    existingCTMuaThietBiYTe.donGia = existingThietBi.donGia;
                    existingCTMuaThietBiYTe.thanhTien += giaLech;
                    existingCTMuaThietBiYTe.CreateBy = userId;

                    existingMaHoaDon.tongTien += giaLech;
                    _context.hoaDonEntities.Update(existingMaHoaDon);

                    _context.thietBiYTeEntities.Update(existingThietBi);

                }

            }

            _context.cTMuaThietBiYTeEntities.Update(existingCTMuaThietBiYTe);
            await _context.SaveChangesAsync();
        }
    }
}
