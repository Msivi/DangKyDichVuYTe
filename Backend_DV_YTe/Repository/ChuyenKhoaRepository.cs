using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class ChuyenKhoaRepository:IChuyenKhoaRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ChuyenKhoaRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateChuyenKhoa(ChuyenKhoaEntity entity)
        {
            var existingChuyenKhoa = await _context.chuyenKhoaEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingChuyenKhoa != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<ChuyenKhoaEntity>(entity);

            await _context.chuyenKhoaEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        public async Task<ChuyenKhoaEntity> DeleteChuyenKhoa(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.chuyenKhoaEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.chuyenKhoaEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.chuyenKhoaEntities.Update(entity);
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

        public async Task<ICollection<ChuyenKhoaEntity>> GetAllChuyenKhoa()
        {
            try
            {
                var entities = await _context.chuyenKhoaEntities
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

        public async Task<ChuyenKhoaEntity> GetChuyenKhoaById(int id)
        {
            var entity = await _context.chuyenKhoaEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task<ICollection<ChuyenKhoaEntity>> SearchChuyenKhoa(string searchKey)
        {
            var ListKH = await _context.chuyenKhoaEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
                c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.tenChuyenKhoa.Contains(searchKey, StringComparison.OrdinalIgnoreCase) 
               
            ) && c.DeletedTime == null).ToList();

            return filteredList;
        }

        public async Task UpdateChuyenKhoa(int id, ChuyenKhoaEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingChuyenKhoa = await _context.chuyenKhoaEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingChuyenKhoa == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingChuyenKhoa.tenChuyenKhoa = entity.tenChuyenKhoa;
            

            _context.chuyenKhoaEntities.Update(existingChuyenKhoa);
            await _context.SaveChangesAsync();
        }
    }
}
