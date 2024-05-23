using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class LoaiThietBiRepository:ILoaiThietBiRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public LoaiThietBiRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateLoaiThietBi(LoaiThietBiEntity entity)
        {
            var existingLoaiThietBi = await _context.loaiThietBiEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingLoaiThietBi != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<LoaiThietBiEntity>(entity);

            await _context.loaiThietBiEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }


        public async Task<LoaiThietBiEntity> DeleteLoaiThietBi(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.loaiThietBiEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy nhà cung cấp!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.loaiThietBiEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.loaiThietBiEntities.Update(entity);
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

        public async Task<ICollection<LoaiThietBiEntity>> GetAllLoaiThietBi()
        {
            try
            {
                var entities = await _context.loaiThietBiEntities
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

        public async Task<LoaiThietBiEntity> GetLoaiThietBiById(int id)
        {
            var entity = await _context.loaiThietBiEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<LoaiThietBiEntity>> SearchLoaiThietBi(string searchKey)
        //{
        //    var ListKH = await _context.loaiThietBiEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c..Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateLoaiThietBi(int id, LoaiThietBiEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingLoaiThietBi = await _context.loaiThietBiEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingLoaiThietBi == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingLoaiThietBi.tenLoaiThietBi = entity.tenLoaiThietBi;

            _context.loaiThietBiEntities.Update(existingLoaiThietBi);
            await _context.SaveChangesAsync();
        }
    }
}
