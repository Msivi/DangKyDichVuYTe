using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Repository
{
    public class LichHenRepository:ILichHenRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public LichHenRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        public async Task<string> CreateLichHen(LichHenEntity entity)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var existingLichBS = await _context.lichHenEntities.Where(c => c.MaBacSi == entity.MaBacSi && c.thoiGianDuKien == entity.thoiGianDuKien && c.DeletedTime == null).ToListAsync();
                if (existingLichBS.Any())
                {
                    throw new Exception("Bác sĩ này đã có ca làm. Vui lòng chọn thời gian khác!");
                }

                var existingBS = await _context.lichHenEntities.Where(c => c.MaBacSi == entity.MaBacSi && c.DeletedTime == null).ToListAsync();
                if(existingBS is null)
                {
                    throw new Exception("Mã bác sĩ không được để trống!");
                }
               

                var existingLoaiThietBi = await _context.lichHenEntities
                   .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

                if (existingLoaiThietBi != null)
                {
                    throw new Exception(message: "Id is already exist!");
                }
                var loaiDV = await _context.dichVuEntities.FirstOrDefaultAsync(c => c.Id == entity.MaDichVu);
                if (loaiDV.MaLoaiDichVu != 1)
                {
                    entity.diaDiem = "https://teams.microsoft.com/l/meetup-join/19:hesjjwTZP907Iy0mhsbdB3D5ZwSu3Q9FDUZRDA1lV_41@thread.tacv2/1714786606106?context=%7B%22Tid%22:%220a3dcbb1-b6f8-47d2-91f3-c215291e6283%22,%22Oid%22:%22dc093a5c-4600-4ee0-b2a6-964d3dd8b7d6%22%7D";
                }
                var mapEntity = _mapper.Map<LichHenEntity>(entity);
                mapEntity.MaKhachHang = userId;
                mapEntity.trangThai = "Chưa khám";

                await _context.lichHenEntities.AddAsync(mapEntity);
                await _context.SaveChangesAsync();

                return mapEntity.Id.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<LichHenEntity> DeleteLichHen(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.lichHenEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.lichHenEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.lichHenEntities.Update(entity);
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

        public async Task<ICollection<LichHenEntity>> GetAllLichHen()
        {
            try
            {
                var entities = await _context.lichHenEntities
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
        public async Task<ICollection<LichHenEntity>> GetAllLichHenKhachHang()
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var entities = await _context.lichHenEntities
                     .Where(c =>c.MaKhachHang==userId && c.DeletedTime == null)

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

        public async Task<ICollection<LichHenEntity>> GetAllLichHenOnline()
        {
            try
            {
                var entities = await _context.lichHenEntities
                                .Include(lh => lh.DichVu)
                                .ThenInclude(dv => dv.LoaiDichVu)
                                .Where(lh => lh.DichVu.LoaiDichVu.tenLoai == "Online")
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

        public async Task<LichHenEntity> GetLichHenById(int id)
        {
            var entity = await _context.lichHenEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<LichHenEntity>> SearchLichHen(string searchKey)
        //{
        //    var ListKH = await _context.lichHenEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (

        //        c.tenLichHen.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.chuyenKhoa.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.bangCap.Contains(searchKey, StringComparison.OrdinalIgnoreCase)

        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateLichHen(int id, LichHenModel entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingLichHen = await _context.lichHenEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingLichHen == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingLichHen.diaDiem = entity.diaDiem;
            existingLichHen.thoiGianDuKien = entity.thoiGianDuKien;
            existingLichHen.MaBacSi = entity.MaBacSi;
            existingLichHen.MaDichVu = entity.MaDichVu;

            existingLichHen.MaKhachHang = userId;


            _context.lichHenEntities.Update(existingLichHen);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLichHenNhanVien(int id, string diaDiem, DateTime thoiGianDuKien)
        {

             

            var existingLichHen = await _context.lichHenEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingLichHen == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingLichHen.diaDiem = diaDiem;
            existingLichHen.thoiGianDuKien = thoiGianDuKien;
           

            existingLichHen.CreateBy = userId;


            _context.lichHenEntities.Update(existingLichHen);
            await _context.SaveChangesAsync();
        }
    }
}
