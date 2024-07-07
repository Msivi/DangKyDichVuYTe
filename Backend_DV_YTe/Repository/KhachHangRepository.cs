using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Backend_DV_YTe.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Backend_DV_YTe.Repository
{
    public class KhachHangRepository:IKhachHangRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        private readonly IDistributedCache _distributedCache;

        public KhachHangRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            
            _distributedCache = distributedCache;
        }
      
        public async Task<ICollection<KhachHangEntity>> GetAllKhachHang()   
        {
            var entities = await _context.khachHangEntities
                .ToListAsync();

            return entities;
        }
        public async Task<ICollection<KhachHangEntity>> SearchKhachHang(string searchKey)
        {
            var ListKH = await _context.khachHangEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
                c.maKhachHang.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.tenKhachHang.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.SDT.Contains(searchKey, StringComparison.OrdinalIgnoreCase)||
                c.CMND.Contains(searchKey, StringComparison.OrdinalIgnoreCase)
            )).ToList();

            return filteredList;
        }
        public async Task<KhachHangEntity> GetKhachHangById(int id)
        {
            var entity = await _context.khachHangEntities.FirstOrDefaultAsync(e => e.maKhachHang == id );
             
            return entity;
        }

        public async Task<KhachHangEntity> GetTTKhachHang()
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);
            var entity = await _context.khachHangEntities.FirstOrDefaultAsync(e => e.maKhachHang == userId);

            return entity;
        }
        public async Task UpdateKhachHang(KhachHangModel entity)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var existingKhachHang = await _context.khachHangEntities
                .FirstOrDefaultAsync(c => c.maKhachHang == userId);

            if (existingKhachHang == null)
            {
                throw new Exception(message: "Account not found!");
            }

            existingKhachHang.SDT = entity.SDT;
            existingKhachHang.CMND = entity.CMND;
            //existingKhachHang.email = entity.email;
            existingKhachHang.tenKhachHang = entity.tenKhachHang;
            existingKhachHang.NgaySinh = entity.NgaySinh;
            existingKhachHang.GioiTinh = entity.GioiTinh;
            _context.khachHangEntities.Update(existingKhachHang);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAvatar(string avatarUrl)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var user = _context.khachHangEntities.FirstOrDefault(p => p.maKhachHang == userId);
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
