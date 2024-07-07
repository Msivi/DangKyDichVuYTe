using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.ObjectModel;

namespace Backend_DV_YTe.Repository
{
    public class DiaChiRepository:IDiaChiRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public DiaChiRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;

        }
        public async Task<string> CreateDiaChi(DiaChiEntity entity)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);


            var existingDiaChi = await _context.diaChiEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingDiaChi != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<DiaChiEntity>(entity);
            mapEntity.KhachHangId = userId;
            await _context.diaChiEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }


        public async Task<DiaChiEntity> DeleteDiaChi(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.diaChiEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.diaChiEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.diaChiEntities.Update(entity);
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

        public async Task<ICollection<DiaChiEntity>> GetAllDiaChi()
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);


                var entities = await _context.diaChiEntities
                     .Where(c =>c.KhachHangId==userId && c.DeletedTime == null)

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

        public async Task<DiaChiEntity> GetDiaChiById(int id)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);


            var entity = await _context.diaChiEntities.FirstOrDefaultAsync(e => e.Id == id && e.KhachHangId== userId && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<DiaChiEntity>> SearchDiaChi(string searchKey)
        //{
        //    var ListKH = await _context.diaChiEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c..Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateDiaChi(int id, DiaChiModel entity)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);
            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingDiaChi = await _context.diaChiEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingDiaChi == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingDiaChi.tenDiaChi = entity.tenDiaChi;
            existingDiaChi.KhachHangId=userId ;
            _context.diaChiEntities.Update(existingDiaChi);
            await _context.SaveChangesAsync();
        }
        public async Task<string> SetDefaultAddressAsync(int addressId)
        {
            // Lấy tất cả các địa chỉ và đặt IsDefault = false
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var allAddresses = await _context.diaChiEntities.Where(c=>c.DeletedTime==null && c.KhachHangId==userId).ToListAsync();
            foreach (var address in allAddresses)
            {
                address.IsDefault = false;
            }

            // Đặt địa chỉ mới là mặc định
            var defaultAddress = await _context.diaChiEntities.FindAsync(addressId);
            if (defaultAddress != null)
            {
                defaultAddress.IsDefault = true;
            }

            await _context.SaveChangesAsync();
            return addressId.ToString();
        }

        public async Task<ICollection<DiaChiEntity>> GetDefaultAddressAsync()
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception("Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var defaultAddress = await _context.diaChiEntities.FirstOrDefaultAsync(a => a.IsDefault && a.KhachHangId == userId && a.DeletedTime == null);
            if (defaultAddress != null)
            {
                return new List<DiaChiEntity> { defaultAddress };
            }

            var addresses = await _context.diaChiEntities.Where(a => a.DeletedTime == null && a.KhachHangId == userId).ToListAsync();
            return addresses;
        }
    }
}
