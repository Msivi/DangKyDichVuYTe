using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class NhaCungCapRepository:INhaCungCapRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public NhaCungCapRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateNhaCungCap(NhaCungCapEntity entity)
        {
            var existingNhaCungCap = await _context.nhaCungCapEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingNhaCungCap != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<NhaCungCapEntity>(entity);

            await _context.nhaCungCapEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        public async Task<NhaCungCapEntity> DeleteNhaCungCap(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.nhaCungCapEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy nhà cung cấp!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.nhaCungCapEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.nhaCungCapEntities.Update(entity);
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

        public async Task<ICollection<NhaCungCapEntity>> GetAllNhaCungCap()
        {
            try
            {
                var entities = await _context.nhaCungCapEntities
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

        public async Task<NhaCungCapEntity> GetNhaCungCapById(int id)
        {
            var entity = await _context.nhaCungCapEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task<ICollection<NhaCungCapEntity>> SearchNhaCungCap(string searchKey)
        {
            var ListKH = await _context.nhaCungCapEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
                c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.tenNhaCungCap.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
            ) && c.DeletedTime == null).ToList();

            return filteredList;
        }

        public async Task UpdateNhaCungCap(int id, NhaCungCapEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingNhaCungCap = await _context.nhaCungCapEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingNhaCungCap == null)
            {
                throw new Exception(message: "Không tìm thấy nhà cung cấp!");
            }
            existingNhaCungCap.tenNhaCungCap = entity.tenNhaCungCap;
            existingNhaCungCap.SDT = entity.SDT;
            existingNhaCungCap.diaChi = entity.diaChi;

            _context.nhaCungCapEntities.Update(existingNhaCungCap);
            await _context.SaveChangesAsync();
        }
    }
}
