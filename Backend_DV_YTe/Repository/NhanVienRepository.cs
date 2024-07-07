using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Repository
{
    public class NhanVienRepository:INhanVienRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        private readonly IDistributedCache _distributedCache;

        public NhanVienRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;

            _distributedCache = distributedCache;
        }




        public async Task<ICollection<NhanVienEntity>> GetAllNhanVien()
        {
            var entities = await _context.nhanVienEntities
                .Where(c=>c.DeletedTime==null)
                .ToListAsync();

            return entities;
        }
        public async Task<ICollection<NhanVienEntity>> SearchNhanVien(string searchKey)
        {
            var ListKH = await _context.nhanVienEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
                c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.tenNhanVien.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.SDT.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.CMND.Contains(searchKey, StringComparison.OrdinalIgnoreCase)
            )).ToList();

            return filteredList;
        }
        public async Task<NhanVienEntity> GetTTNhanVien()
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);
            var entity = await _context.nhanVienEntities.FirstOrDefaultAsync(e => e.Id == userId);

            return entity;
        }
        public async Task<NhanVienEntity> GetNhanVienById(int id)
        {
            var entity = await _context.nhanVienEntities.FirstOrDefaultAsync(e => e.Id == id);

            return entity;
        }
        public async Task UpdateNhanVien(NhanVienModel entity)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var existingNhanVien = await _context.nhanVienEntities
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (existingNhanVien == null)
            {
                throw new Exception(message: "Account not found!");
            }

            existingNhanVien.SDT = entity.SDT;
            existingNhanVien.CMND = entity.CMND;
            //existingKhachHang.email = entity.email;
            existingNhanVien.tenNhanVien = entity.tenNhanVien;

            _context.nhanVienEntities.Update(existingNhanVien);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAvatar(string avatarUrl)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var user = _context.nhanVienEntities.FirstOrDefault(p => p.Id == userId);
                if (user != null)
                {
                    user.Avatar = avatarUrl;
                    await _context.SaveChangesAsync(); // Sử dụng SaveChangesAsync() để lưu thay đổi vào cơ sở dữ liệu
                }
            }
            catch (Exception ex)
            {
                throw; // Không cần throw ex; vì sẽ mất thông tin gốc về exception
            }
        }
    }
}
