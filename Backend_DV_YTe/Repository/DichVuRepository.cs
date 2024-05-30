using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Repository
{
    public class DichVuRepository:IDichVuRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DichVuRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache, IWebHostEnvironment webHostEnvironment)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> CreateDichVu(DichVuEntity entity, IFormFile imageFile)
        {
            var existingDichVu = await _context.dichVuEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingDichVu != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            entity.hinhAnh = $"/Images/" + uniqueFileName;

            await _context.dichVuEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.Id.ToString();
        }

        public async Task<DichVuEntity> DeleteDichVu(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.dichVuEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy nhà cung cấp!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.dichVuEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.dichVuEntities.Update(entity);
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

        public async Task<ICollection<DichVuEntity>> GetAllDichVu()
        {
            try
            {
                var entities = await _context.dichVuEntities
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

        public async Task<ICollection<DichVuTheoLoaiModel>> GetDichVuByLoaiDichVuAsync(int? loaiDichVuId, int chuyenKhoaId)
        {
            try
            {
                if(chuyenKhoaId == 0)
                {
                    throw new Exception(message: "Hãy chọn chuyên khoa");
                }

                var query = _context.dichVuEntities.AsQueryable();

                if (loaiDichVuId.HasValue)
                {
                    query = query.Where(dv => dv.MaLoaiDichVu == loaiDichVuId.Value);
                }

                 
               query = query.Where(dv => dv.MaChuyenKhoa == chuyenKhoaId);
               

                var dichVuTheoLoaiModels = await query
                    .Select(dv => new DichVuTheoLoaiModel
                    {
                        MaDichVu=dv.Id,
                        TenDichVu = dv.tenDichVu,
                        Gia = dv.gia,
                        MoTa = dv.moTa,
                        TenChuyenKhoa = dv.ChuyenKhoa.tenChuyenKhoa,
                       HinhAnh=dv.hinhAnh,
                         
                    })
                    .ToListAsync();

                return dichVuTheoLoaiModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<DichVuEntity>> GetAllDichVuTheoChuyenKhoa(string ChuyenKhoa)
        {
            try
            {
                var query = from dichVu in _context.dichVuEntities
                            //join ctBacSi in _context.cTBacSiEntities on dichVu.Id equals ctBacSi.MaDichVu
                            //join bacSi in _context.BacSiEntities on ctBacSi.MaBacSi equals bacSi.Id
                            //where ctBacSi.chuyenKhoa == ChuyenKhoa
                            select dichVu;

                var danhSachDichVuTheoChuyenKhoa = await query.ToListAsync();
                if (danhSachDichVuTheoChuyenKhoa is null)
                {
                    throw new Exception("Empty list!");
                }
                return danhSachDichVuTheoChuyenKhoa;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DichVuEntity> GetDichVuById(int id)
        {
            var entity = await _context.dichVuEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task<ICollection<DichVuEntity>> SearchDichVu(string searchKey)
        {
            var ListKH = await _context.dichVuEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
                c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.tenDichVu.Contains(searchKey, StringComparison.OrdinalIgnoreCase)
                //c.nhaSanXuat.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
            ) && c.DeletedTime == null).ToList();

            return filteredList;
        }

        public async Task UpdateDichVu(int id, DichVuModel entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingDichVu = await _context.dichVuEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingDichVu == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingDichVu.tenDichVu=entity.tenDichVu;
            existingDichVu.moTa= entity.moTa;
            existingDichVu.gia= entity.gia;
            existingDichVu.MaLoaiDichVu = entity.MaLoaiDichVu;
            existingDichVu.MaChuyenKhoa=entity.MaChuyenKhoa;
            existingDichVu.CreateBy = userId;
            //var mapEntity = _mapper.Map<DichVuEntity>(entity);
            //mapEntity.CreateBy = userId;

            _context.dichVuEntities.Update(existingDichVu);
            await _context.SaveChangesAsync();
        }

        // thống kê tổng giá trị của các dịch vụ đã được đặt trong khoảng
        public async Task<double> thongKeTinhTongGiaTriDichVu(DateTime fromDate, DateTime toDate)
        {
            var thongke = _context.dichVuEntities
                            .Join(
                                _context.lichHenEntities,
                                dichVu => dichVu.Id, // Khóa chính trong bảng DichVu
                                lichHen => lichHen.MaDichVu, // Khóa ngoại trong bảng LichHen liên kết với DichVu
                                (dichVu, lichHen) => new { DichVu = dichVu, LichHen = lichHen }
                            )
                            .Where(x => x.LichHen.thoiGianDuKien >= fromDate && x.LichHen.thoiGianDuKien <= toDate)
                            .Sum(x => x.DichVu.gia);

          
            return thongke;
        }
        //public async Task<double> TinhGiaTriTrungBinhDichVuTrongKhoangThoiGian(DateTime ngayBD, DateTime ngayKT)
        //{
        //    return await _context.dichVuEntities
        //        .Where(d => d.LichHen.Any(lh => lh.thoiGianDuKien >= ngayBD && lh.thoiGianDuKien <= ngayKT))
        //        .AverageAsync(d => d.gia);
        //}
        public async Task<List<DichVuSuDung>> TimDichVuDaSuDung(DateTime fromDate, DateTime toDate)
        {
            var dichVuDaSuDung = await _context.dichVuEntities
                                .Join(
                                    _context.lichHenEntities,
                                    dichVu => dichVu.Id, // Khóa chính của bảng DichVuEntities
                                    lichHen => lichHen.MaDichVu, // Khóa ngoại trong bảng LichHen liên kết với DichVuEntities
                                    (dichVu, lichHen) => new { DichVu = dichVu, LichHen = lichHen }
                                )
                                .Where(d => d.LichHen.thoiGianDuKien >= fromDate && d.LichHen.thoiGianDuKien <= toDate)
                                .GroupBy(d => d.DichVu.tenDichVu)
                                .Select(g => new DichVuSuDung { TenDichVu = g.Key, SoLuong =(int) g.Select(x => x.DichVu.gia).Sum() })
                                .ToListAsync();

            return dichVuDaSuDung;
        }

        //public async Task<int> TimDichVuGiaTriThapNhat(DateTime fromDate, DateTime toDate)
        //{
        //    double maxGia = await _context.dichVuEntities
        //         .Where(d => d.LichHen.Any(lh => lh.thoiGianDuKien >= fromDate && lh.thoiGianDuKien <= toDate))
        //         .MinAsync(d => d.gia);

        //    int count = await _context.dichVuEntities
        //        .CountAsync(d => d.gia == maxGia && d.LichHen.Any(lh => lh.thoiGianDuKien >= fromDate && lh.thoiGianDuKien <= toDate));

        //    return count;
        //}
    }
}
