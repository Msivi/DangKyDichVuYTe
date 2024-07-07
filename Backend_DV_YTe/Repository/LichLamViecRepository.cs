using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Repository
{
    public class LichLamViecRepository:ILichLamViecRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        private readonly IDistributedCache _distributedCache;

        public LichLamViecRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;

            _distributedCache = distributedCache;
        }

         
        public async Task<ICollection<LichLamViecEntity>> GetAllLichLamViec()
        {
            var entities = await _context.lichLamViecEntities
                .Where(c=>c.DeletedTime==null)
                .ToListAsync();

            return entities;
        }
        public async Task<string> CreateLichLamViec(LichLamViecEntity entity)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var existingLichLamViec = await _context.lichLamViecEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingLichLamViec != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<LichLamViecEntity>(entity);
            mapEntity.CreateBy = userId;
            await _context.lichLamViecEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }
        public async Task<ICollection<LichLamViecEntity>> GetTTLichLamViecBacSi()
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var entity = await _context.lichLamViecEntities
                .Where(e => e.MaBacSi == userId && e.DeletedTime == null)
                .ToListAsync();

            return entity;
        }
        public async Task<LichLamViecEntity> GetLichLamViecById(int id)
        {
            var entity = await _context.lichLamViecEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime==null);
            if(entity==null)
            {
                throw new Exception(message: "Không tìm thấy");
            }    
            return entity;
        }
        public async Task<ICollection<LichLamViecEntity>> GetLichLamViecByIdBacSi(int id)
        {
            var entity = await _context.lichLamViecEntities.Where(e => e.MaBacSi == id && e.DeletedTime == null).ToListAsync();
            if (entity == null)
            {
                throw new Exception(message: "Không tìm thấy");
            }
            return entity;
        }
        public async Task UpdateLichLamViec(int id, LichLamViecModel entity)
        {
            
            var existingLichLamViec = await _context.lichLamViecEntities
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingLichLamViec == null)
            {
                throw new Exception(message: "Account not found!");
            }

            existingLichLamViec.Ngay = entity.Ngay;
            existingLichLamViec.GioBatDau = entity.GioBatDau;
            existingLichLamViec.GioKetThuc = entity.GioKetThuc;
            existingLichLamViec.MaBacSi = entity.MaBacSi;

            _context.lichLamViecEntities.Update(existingLichLamViec);
            await _context.SaveChangesAsync();
        }
        public async Task<LichLamViecEntity> DeleteLichLamViec(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.lichLamViecEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.lichLamViecEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.lichLamViecEntities.Update(entity);
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
    }
}
