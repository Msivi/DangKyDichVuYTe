using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class CTBacSiRepository: ICTBacSiRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CTBacSiRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateCTBacSi(CTBacSiEntity ctBacSi)
        {
            var existingBacSi = _context.BacSiEntities.FirstOrDefault(c => c.Id == ctBacSi.MaBacSi && c.DeletedTime == null);
            if (existingBacSi == null)
            {
                throw new Exception("Không tìm thấy Id bác sĩ!");
            }
            var existingChuyenKhoa = _context.chuyenKhoaEntities.FirstOrDefault(c => c.Id == ctBacSi.MaChuyenKhoa && c.DeletedTime == null);
            if (existingChuyenKhoa == null)
            {
                throw new Exception("Không tìm thấy Id chuyên khoa!");
            }
         
            await _context.cTBacSiEntities.AddAsync(ctBacSi);
       

            await _context.SaveChangesAsync();

            return $"{ctBacSi.MaBacSi}-{ctBacSi.MaChuyenKhoa}";
        }

        public async Task<CTBacSiEntity> DeleteCTBacSi(int maBS, int maCK, bool isPhysical)
        {
            try
            {
                var entity = await _context.cTBacSiEntities.FirstOrDefaultAsync(c => c.MaBacSi == maBS && c.MaChuyenKhoa == maCK && c.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy Id chi tiết bác sĩ!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.cTBacSiEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.cTBacSiEntities.Update(entity);
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

        public async Task<ICollection<CTBacSiEntity>> GetAllCTBacSi()
        {
            try
            {
                var entities = await _context.cTBacSiEntities
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

        public async Task<CTBacSiEntity> GetCTBacSiById(int maBS, int maCK)
        {
            var entity = await _context.cTBacSiEntities.FirstOrDefaultAsync(c => c.MaBacSi == maBS && c.MaChuyenKhoa == maCK && c.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

         
        public async Task UpdateCTBacSi(int maBS, int maCK, CTBacSiEntity entity)
        {

            var existingBacSi = _context.BacSiEntities.FirstOrDefault(c => c.Id == maBS  && c.DeletedTime == null);
            if (existingBacSi == null)
            {
                throw new Exception("Không tìm thấy Id bác sĩ!");
            }
            var existingChuyenKhoa = _context.chuyenKhoaEntities.FirstOrDefault(c => c.Id == maCK && c.DeletedTime == null);
            if (existingChuyenKhoa == null)
            {
                throw new Exception("Không tìm thấy Id chuyên khoa!");
            }
            var ctBS= new CTBacSiEntity()
            {
                MaBacSi= maBS,
                MaChuyenKhoa= maCK,
            };
            _context.cTBacSiEntities.Update(ctBS);
            await _context.SaveChangesAsync();
        }
    }
}
