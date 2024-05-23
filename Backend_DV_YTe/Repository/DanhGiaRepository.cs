using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Backend_DV_YTe.Repository
{
    public class DanhGiaRepository : IDanhGiaRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DanhGiaRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache, IWebHostEnvironment webHostEnvironment)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _webHostEnvironment = webHostEnvironment;
        }
        //public async Task<string> CreateDanhGia(DanhGiaEntity entity)
        //{
        //    var existingDanhGia = await _context.danhGiaEntities
        //      .FirstOrDefaultAsync(c => c.MaKetQuaDichVu == entity.MaKetQuaDichVu && (c.DeletedTime == null || c.DeletedTime != null));

        //    if (existingDanhGia != null)
        //    {
        //        throw new Exception(message: "Bạn đã thực hiện đánh giá dịch vụ này rồi!");
        //    }

        //    //var mapEntity = _mapper.Map<DanhGiaEntity>(entity);

        //    await _context.danhGiaEntities.AddAsync(entity);
        //    await _context.SaveChangesAsync();

        //    return entity.Id.ToString();
        //}

        public async Task<string> CreateDanhGia(DanhGiaEntity entity, IFormFile imageFile)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId"); // Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var existingDanhGia = await _context.danhGiaEntities
                .FirstOrDefaultAsync(c => c.MaKetQuaDichVu == entity.MaKetQuaDichVu && (c.DeletedTime == null || c.DeletedTime != null));
            if (existingDanhGia != null)
            {
                throw new Exception(message: "Bạn đã thực hiện đánh giá dịch vụ này rồi!");
            }
            if(entity.soSaoDanhGia>5|| entity.soSaoDanhGia < 1)
            {
                throw new Exception(message: "Đánh giá nằm trong khoản 1 đến 5 sao!");
            }

            var query = from ketQuaDichVu in _context.ketQuaDichVuEntities
                        where ketQuaDichVu.CreateBy == userId
                        select new
                        {
                            ketQuaDichVu.Id,
                            ketQuaDichVu.CreateBy
                        };
            var danhSach = await query.ToListAsync();
            var ktTonTai = danhSach.Any(dk => dk.Id == entity.MaKetQuaDichVu);




            if (!ktTonTai)
            {
                throw new Exception(message: "Mã đánh giá không hợp lệ!");
            }

            var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            entity.hinhAnh = $"/Images/" + uniqueFileName;

            _context.danhGiaEntities.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id.ToString();
        }

        public async Task<DanhGiaEntity> DeleteDanhGia(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.danhGiaEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.danhGiaEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.danhGiaEntities.Update(entity);
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

        public async Task<ICollection<DanhGiaEntity>> GetAllDanhGiaChuaDuyet()
        {
            try
            {
                var entities = await _context.danhGiaEntities
                     .Where(c =>c.trangThai=="0" &&c.DeletedTime == null)

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
        public async Task<ICollection<DanhGiaEntity>> GetAllDanhGiaDaDuyet()
        {
            try
            {
                var entities = await _context.danhGiaEntities
                     .Where(c => c.trangThai == "1" && c.DeletedTime == null)

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
        public async Task<DanhGiaEntity> GetDanhGiaById(int id)
        {
            var entity = await _context.danhGiaEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<DanhGiaEntity>> SearchDanhGia(string searchKey)
        //{
        //    var ListKH = await _context.danhGiaEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (

        //        c.tenDanhGia.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.chuyenKhoa.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.bangCap.Contains(searchKey, StringComparison.OrdinalIgnoreCase)

        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateDanhGia(int id, DanhGiaModel entity, IFormFile imageFile)
        {
            if (entity == null)
            {
                throw new Exception("The entity field is required!");
            }

            var existingDanhGia = await _context.danhGiaEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingDanhGia == null)
            {
                throw new Exception("Không tìm thấy!");
            }

            byte[] userIdBytes = await _distributedCache.GetAsync("UserId"); // Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingDanhGia.noiDungDanhGia = entity.noiDungDanhGia;
            existingDanhGia.soSaoDanhGia = entity.soSaoDanhGia;
            existingDanhGia.CreateBy = userId;

            if (imageFile != null)
            {
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                if (!string.IsNullOrEmpty(existingDanhGia.hinhAnh))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.ContentRootPath, existingDanhGia.hinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                existingDanhGia.hinhAnh = $"/Images/{uniqueFileName}";
            }

            _context.danhGiaEntities.Update(existingDanhGia);
            await _context.SaveChangesAsync();
        }

        public async Task DuyetDanhGia(List<int>ids)
        {
            try
            {
                //List<int> ids = idList.Split(',').Select(int.Parse).ToList();
                var existingDanhGia = await _context.danhGiaEntities
                                       .Where(c => ids.Contains(c.Id) && c.DeletedTime == null)
                                       .ToListAsync();
                if (existingDanhGia is null)
                {
                    throw new Exception("Đánh giá không tồn tại");
                }
                foreach (var danhGia in existingDanhGia)
                {
                    danhGia.trangThai = "1";
                    _context.danhGiaEntities.Update(danhGia);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DichVuDanhGia>> GetDichVuDanhGia(int maDichVu)
        {
            var danhGiaDichVu = await _context.danhGiaEntities
                .Join(
                    _context.ketQuaDichVuEntities,
                    danhGia => danhGia.MaKetQuaDichVu,
                    ketQuaDichVu => ketQuaDichVu.Id,
                    (danhGia, ketQuaDichVu) => new { DanhGia = danhGia, KetQuaDichVu = ketQuaDichVu }
                )
                .Join(
                    _context.lichHenEntities,
                    d => d.KetQuaDichVu.MaLichHen,
                    lichHen => lichHen.Id,
                    (d, lichHen) => new { DanhGia = d.DanhGia, KetQuaDichVu = d.KetQuaDichVu, LichHen = lichHen }
                )
                .Where(d => d.LichHen.DichVu.Id == maDichVu)
                .Select(d => new { MaDichVu = d.LichHen.DichVu.Id, SoSaoDanhGia = d.DanhGia.soSaoDanhGia })
                .ToListAsync();

            var dichVuDanhGiaList = danhGiaDichVu
                .GroupBy(d => d.MaDichVu)
                .Select(g => new DichVuDanhGia { MaDichVu = g.Key, SoSaoTBDanhGia = g.Average(x => x.SoSaoDanhGia), SoDanhGia=g.Count() })
                .ToList();

            return dichVuDanhGiaList;
        }
        public async Task<List<DanhGiaEntity>> GetAllDanhGiaByMaDichVu(int maDichVu)
        {
            var danhGiaList = await _context.danhGiaEntities
                .Join(
                    _context.ketQuaDichVuEntities,
                    danhGia => danhGia.MaKetQuaDichVu,
                    ketQuaDichVu => ketQuaDichVu.Id,
                    (danhGia, ketQuaDichVu) => new { DanhGia = danhGia, KetQuaDichVu = ketQuaDichVu }
                )
                .Join(
                    _context.lichHenEntities,
                    d => d.KetQuaDichVu.MaLichHen,
                    lichHen => lichHen.Id,
                    (d, lichHen) => new { DanhGia = d.DanhGia, KetQuaDichVu = d.KetQuaDichVu, LichHen = lichHen }
                )
                .Where(d => d.LichHen.DichVu.Id == maDichVu)
                .Select(d => d.DanhGia)
                .ToListAsync();

            return danhGiaList;
        }
    }
}

