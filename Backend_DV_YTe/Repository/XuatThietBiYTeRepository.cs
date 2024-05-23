using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class XuatThietBiYTeRepository:IXuatThietBiYTeRepository 
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public XuatThietBiYTeRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateXuatThietBiYTe(XuatThietBiYTeEntity entity)
        {
            var existingXuatThietBiYTe = await _context.xuatThietBiYTeEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingXuatThietBiYTe != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<XuatThietBiYTeEntity>(entity);

            await _context.xuatThietBiYTeEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        public async Task<XuatThietBiYTeEntity> DeleteXuatThietBiYTe(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.xuatThietBiYTeEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy ID!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.xuatThietBiYTeEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.xuatThietBiYTeEntities.Update(entity);
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

        public async Task<ICollection<XuatThietBiYTeEntity>> GetAllXuatThietBiYTe()
        {
            try
            {
                var entities = await _context.xuatThietBiYTeEntities
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

        public async Task<XuatThietBiYTeEntity> GetXuatThietBiYTeById(int id)
        {
            var entity = await _context.xuatThietBiYTeEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<XuatThietBiYTeEntity>> SearchXuatThietBiYTe(string searchKey)
        //{
        //    var ListKH = await _context.xuatThietBiYTeEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c..Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateXuatThietBiYTe(int id, XuatThietBiYTeEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingXuatThietBiYTe = await _context.xuatThietBiYTeEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingXuatThietBiYTe == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingXuatThietBiYTe.ngayTao = entity.ngayTao;



            _context.xuatThietBiYTeEntities.Update(existingXuatThietBiYTe);
            await _context.SaveChangesAsync();
        }
    }
}
