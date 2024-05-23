using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class ThietBiYTeRepository:IThietBiYteRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ThietBiYTeRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateThietBiYTe(ThietBiYTeEntity entity)
        {
            var existingThietBiYTe = await _context.thietBiYTeEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingThietBiYTe != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<ThietBiYTeEntity>(entity);

            await _context.thietBiYTeEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        public async Task<ThietBiYTeEntity> DeleteThietBiYTe(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.thietBiYTeEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.thietBiYTeEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.thietBiYTeEntities.Update(entity);
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

        public async Task<ICollection<ThietBiYTeEntity>> GetAllThietBiYTe()
        {
            try
            {
                var entities = await _context.thietBiYTeEntities
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

        public async Task<ThietBiYTeEntity> GetThietBiYTeById(int id)
        {
            var entity = await _context.thietBiYTeEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task<ICollection<ThietBiYTeEntity>> SearchThietBiYTe(string searchKey)
        {
            var ListKH = await _context.thietBiYTeEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
                c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.tenThietBi.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.nhaSanXuat.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
            ) && c.DeletedTime == null).ToList();

            return filteredList;
        }

        public async Task UpdateThietBiYTe(int id, ThietBiYTeEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingThietBiYTe = await _context.thietBiYTeEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingThietBiYTe == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingThietBiYTe.tenThietBi = entity.tenThietBi;
            existingThietBiYTe.donGia = entity.donGia;
            existingThietBiYTe.soLuong = entity.soLuong;
            existingThietBiYTe.ngaySanXuat = entity.ngaySanXuat;
            existingThietBiYTe.ngayHetHan = entity.ngayHetHan;
            existingThietBiYTe.nhaSanXuat = entity.nhaSanXuat;
            existingThietBiYTe.moTa = entity.moTa;

            _context.thietBiYTeEntities.Update(existingThietBiYTe);
            await _context.SaveChangesAsync();
        }
    }
}
