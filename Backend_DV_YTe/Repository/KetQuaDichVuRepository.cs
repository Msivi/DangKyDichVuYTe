using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Backend_DV_YTe.Repository
{
    public class KetQuaDichVuRepository:IKetQuaDichVuRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public KetQuaDichVuRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        public async Task<string> CreateKetQuaDichVu(KetQuaDichVuEntity entity)
        {
            var existingKetQuaDichVu = await _context.ketQuaDichVuEntities
                .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingKetQuaDichVu != null)
            {
                throw new Exception(message: "Id already exists!");
            }

            var lichHen = await _context.lichHenEntities.FirstOrDefaultAsync(c => c.Id == entity.MaLichHen && c.DeletedTime == null);
            if (lichHen is null)
            {
                throw new Exception(message: "Lich hen Id not found!");
            }

            lichHen.trangThai = "Đã khám";
            await _context.SaveChangesAsync();

            var mapEntity = _mapper.Map<KetQuaDichVuEntity>(entity);
            //ket qua của khách hàng nào
            mapEntity.CreateBy = lichHen.MaKhachHang;

            await _context.ketQuaDichVuEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }


        public async Task<KetQuaDichVuEntity> DeleteKetQuaDichVu(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.ketQuaDichVuEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.ketQuaDichVuEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.ketQuaDichVuEntities.Update(entity);
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

        public async Task<ICollection<KetQuaDichVuEntity>> GetAllKetQuaDichVu()
        {
            try
            {
                var entities = await _context.ketQuaDichVuEntities
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

        public async Task<ICollection<TTKetQuaDichVuKhachHangModel>> GetKetQuaDichVuByKhachHang()
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var result = await (from ketQua in _context.ketQuaDichVuEntities
                                join lichHen in _context.lichHenEntities on ketQua.MaLichHen equals lichHen.Id
                                join bacSi in _context.BacSiEntities on lichHen.MaBacSi equals bacSi.Id
                                join dichVu in _context.dichVuEntities on lichHen.MaDichVu equals dichVu.Id
                                where lichHen.MaKhachHang == userId
                                select new TTKetQuaDichVuKhachHangModel
                                {
                                    maKetQua=ketQua.Id.ToString(),
                                    moTa = ketQua.moTa,
                                    diaDiem = lichHen.diaDiem,
                                    tenBacSi = bacSi.tenBacSi,
                                    tenDichVu = dichVu.tenDichVu
                                }).ToListAsync();

            return result;
        }

        public async Task<KetQuaDichVuEntity> GetKetQuaDichVuById(int id)
        {
            
            var entity = await _context.ketQuaDichVuEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<KetQuaDichVuEntity>> SearchKetQuaDichVu(string searchKey)
        //{
        //    var ListKH = await _context.ketQuaDichVuEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c..Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateKetQuaDichVu(int id, KetQuaDichVuModel entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingKetQua = await _context.ketQuaDichVuEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingKetQua == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingKetQua.moTa = entity.moTa;


            existingKetQua.CreateBy = userId;


            _context.ketQuaDichVuEntities.Update(existingKetQua);
            await _context.SaveChangesAsync();
        }
    }
}
