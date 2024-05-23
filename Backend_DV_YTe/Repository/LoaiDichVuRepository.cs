using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace Backend_DV_YTe.Repository
{
    public class LoaiDichVuRepository:ILoaiDichVuRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public LoaiDichVuRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateLoaiDichVu(LoaiDichVuEntity entity)
        {
            var existingLoaiDichVu = await _context.loaiDichVuEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingLoaiDichVu != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<LoaiDichVuEntity>(entity);

            await _context.loaiDichVuEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }
     

        public async Task<LoaiDichVuEntity> DeleteLoaiDichVu(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.loaiDichVuEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy nhà cung cấp!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.loaiDichVuEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.loaiDichVuEntities.Update(entity);
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

        public async Task<ICollection<LoaiDichVuEntity>> GetAllLoaiDichVu()
        {
            try
            {
                var entities = await _context.loaiDichVuEntities
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

        public async Task<LoaiDichVuEntity> GetLoaiDichVuById(int id)
        {
            var entity = await _context.loaiDichVuEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<LoaiDichVuEntity>> SearchLoaiDichVu(string searchKey)
        //{
        //    var ListKH = await _context.loaiDichVuEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c..Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateLoaiDichVu(int id, LoaiDichVuEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingLoaiDichVu = await _context.loaiDichVuEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingLoaiDichVu == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingLoaiDichVu.tenLoai = entity.tenLoai;

            _context.loaiDichVuEntities.Update(existingLoaiDichVu);
            await _context.SaveChangesAsync();
        }
    }
}
