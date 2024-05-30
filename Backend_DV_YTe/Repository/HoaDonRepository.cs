using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Repository
{
    public class HoaDonRepository: IHoaDonRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public HoaDonRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        public async Task<string> CreateHoaDon()
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId"); // Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

           
            var newHoaDon = new HoaDonEntity
            {
                ngayMua = DateTime.Now,
                tongTien = 0,
                trangThai = "False",
                MaKhachHang = userId
            };

            await _context.hoaDonEntities.AddAsync(newHoaDon);
            await _context.SaveChangesAsync();

            return newHoaDon.Id.ToString();
        }
        public async Task UpdateHoaDon(int id, HoaDonEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingHoaDon = await _context.hoaDonEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingHoaDon == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingHoaDon.ngayMua = DateTime.Now;
            //existingHoaDon.

            //existingHoaDon.MaKhachHang = userId;


            _context.hoaDonEntities.Update(existingHoaDon);
            await _context.SaveChangesAsync();
        }

        public async Task<HoaDonEntity> DeleteHoaDon(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.hoaDonEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.hoaDonEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.hoaDonEntities.Update(entity);
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

        public async Task<ICollection<HoaDonEntity>> GetAllHoaDon()
        {
            try
            {
                var entities = await _context.hoaDonEntities
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
        public ICollection<HoaDonEntity> GetHoaDonByMaKhachHang(int userId)
        {

            var hoaDonList = _context.hoaDonEntities
                .Include(h => h.CTMuaThuoc)
                .Include(h => h.CTMuaThietBiYTe)
                .Where(h => h.MaKhachHang == userId)
                .ToList();

            return hoaDonList;
        }
    }
}
