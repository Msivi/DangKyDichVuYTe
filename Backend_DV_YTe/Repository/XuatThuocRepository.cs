using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class XuatThuocRepository:IXuatThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public XuatThuocRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateXuatThuoc(XuatThuocEntity entity)
        {
            var existingXuatThuoc = await _context.xuatThuocEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingXuatThuoc != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<XuatThuocEntity>(entity);

            await _context.xuatThuocEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        public async Task<XuatThuocEntity> DeleteXuatThuoc(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.xuatThuocEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy ID!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.xuatThuocEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.xuatThuocEntities.Update(entity);
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

        public async Task<ICollection<XuatThuocEntity>> GetAllXuatThuoc()
        {
            try
            {
                var entities = await _context.xuatThuocEntities
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

        public async Task<XuatThuocEntity> GetXuatThuocById(int id)
        {
            var entity = await _context.xuatThuocEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<XuatThuocEntity>> SearchXuatThuoc(string searchKey)
        //{
        //    var ListKH = await _context.xuatThuocEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c..Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateXuatThuoc(int id, XuatThuocEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingXuatThuoc = await _context.xuatThuocEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingXuatThuoc == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingXuatThuoc.ngayTao = entity.ngayTao;
            


            _context.xuatThuocEntities.Update(existingXuatThuoc);
            await _context.SaveChangesAsync();
        }
    }
}
