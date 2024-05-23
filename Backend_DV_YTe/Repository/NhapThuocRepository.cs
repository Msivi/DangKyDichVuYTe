using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class NhapThuocRepository:INhapThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public NhapThuocRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateNhapThuoc(NhapThuocEntity entity)
        {
            var existingNhapThuoc = await _context.nhapThuocEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingNhapThuoc != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<NhapThuocEntity>(entity);

            await _context.nhapThuocEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        public async Task<NhapThuocEntity> DeleteNhapThuoc(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.nhapThuocEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.nhapThuocEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.nhapThuocEntities.Update(entity);
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

        public async Task<ICollection<NhapThuocEntity>> GetAllNhapThuoc()
        {
            try
            {
                var entities = await _context.nhapThuocEntities
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

        public async Task<NhapThuocEntity> GetNhapThuocById(int id)
        {
            var entity = await _context.nhapThuocEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<NhapThuocEntity>> SearchNhapThuoc(string searchKey)
        //{
        //    var ListKH = await _context.nhapThuocEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c..Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateNhapThuoc(int id, NhapThuocEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingNhapThuoc = await _context.nhapThuocEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingNhapThuoc == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingNhapThuoc.ngayTao = entity.ngayTao;
            existingNhapThuoc.MaNhaCungCap = entity.MaNhaCungCap;
            

            _context.nhapThuocEntities.Update(existingNhapThuoc);
            await _context.SaveChangesAsync();
        }
    }
}
