using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;

namespace Backend_DV_YTe.Repository
{
    public class BacSiRepository:IBacSiRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly IWebHostEnvironment _webHostEnvironment;
         
        public BacSiRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache, IWebHostEnvironment webHostEnvironment)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> CreateBacSi(BacSiEntity entity, IFormFile imageFile)
        {
            var existingBacSi = await _context.BacSiEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingBacSi != null)
            {
                throw new Exception(message: "Id is already exist!");
            }
            var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            entity.hinhAnh = $"/Images/" + uniqueFileName;
            //var mapEntity = _mapper.Map<BacSiEntity>(entity);

            await _context.BacSiEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.Id.ToString();
        }

        public async Task<BacSiEntity> DeleteBacSi(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.BacSiEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.BacSiEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.BacSiEntities.Update(entity);
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

        public async Task<ICollection<BacSiEntity>> GetAllBacSi()
        {
            try
            {
                var entities = await _context.BacSiEntities
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

        public async Task<ICollection<BacSiInfoModel>> GetAllTTBacSi()
        {
            try
            {
                var danhSachBacSi = await _context.BacSiEntities
                                    .Include(bs => bs.CTBacSi)
                                    .ThenInclude(ct => ct.ChuyenKhoa)
                                    .Where(c => c.DeletedTime == null)
                                    .ToListAsync();

                if (danhSachBacSi is null)
                {
                    throw new Exception("Empty list!");
                }

                var bacSiInfoList = danhSachBacSi.Select(bs => new BacSiInfoModel
                {
                    Id = bs.Id,
                    TenBacSi = bs.tenBacSi,
                    Email=bs.email,
                    BangCap = bs.bangCap,
                    HinhAnh=bs.hinhAnh,
                    ChuyenKhoa = bs.CTBacSi.Select(ct => ct.ChuyenKhoa.tenChuyenKhoa).ToList()
                }).ToList();

                return bacSiInfoList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BacSiInfoModel> GetAllTTBacSiById()
        {
            try
            {
                 
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception("Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var bacSi = await _context.BacSiEntities
                                    .Include(bs => bs.CTBacSi)
                                    .ThenInclude(ct => ct.ChuyenKhoa)
                                    .Where(c => c.DeletedTime == null && c.Id == userId)
                                    .FirstOrDefaultAsync();

               
                if (bacSi == null)
                {
                    throw new Exception("Bác sĩ không tồn tại!");
                }

            
                var bacSiInfo = new BacSiInfoModel
                {
                    Id = bacSi.Id,
                    TenBacSi = bacSi.tenBacSi,
                    Email = bacSi.email,
                    BangCap = bacSi.bangCap,
                    HinhAnh = bacSi.hinhAnh,
                    ChuyenKhoa = bacSi.CTBacSi.Select(ct => ct.ChuyenKhoa.tenChuyenKhoa).ToList()
                };

                return bacSiInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ICollection<BacSiInfoModel>> GetBacSiByChuyenKhoa(string tenChuyenKhoa)
        {
            try
            {
                var danhSachBacSi = await _context.BacSiEntities
                    .Include(bs => bs.CTBacSi)
                    .ThenInclude(ct => ct.ChuyenKhoa)
                    .Where(bs => bs.CTBacSi.Any(ct => ct.ChuyenKhoa.tenChuyenKhoa == tenChuyenKhoa) && bs.DeletedTime==null)
                    .ToListAsync();

                var bacSiInfoList = danhSachBacSi.Select(bs => new BacSiInfoModel
                {
                    Id = bs.Id,
                    TenBacSi = bs.tenBacSi,
                    BangCap = bs.bangCap,
                    ChuyenKhoa = bs.CTBacSi.Select(ct => ct.ChuyenKhoa.tenChuyenKhoa).ToList()
                }).ToList();
                return bacSiInfoList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BacSiEntity> GetBacSiById(int id)
        {
            var entity = await _context.BacSiEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task<ICollection<BacSiEntity>> SearchBacSi(string searchKey)
        {
            var ListKH = await _context.BacSiEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
              
                c.tenBacSi.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                //c.chuyenKhoa.Contains(searchKey, StringComparison.OrdinalIgnoreCase)||
                c.bangCap.Contains(searchKey, StringComparison.OrdinalIgnoreCase)
             
            ) && c.DeletedTime == null).ToList();

            return filteredList;
        }

        public async Task UpdateBacSi(int id, UpdateBacSiModel entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingBacSi = await _context.BacSiEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            //if (existingBacSi == null)
            //{
            //    throw new Exception(message: "Không tìm thấy!");
            //}
            //var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            //var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

            //using (var fileStream = new FileStream(imagePath, FileMode.Create))
            //{
            //    await imageFile.CopyToAsync(fileStream);
            //}



            //byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            //if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            //{
            //    throw new Exception(message: "Vui lòng đăng nhập!");
            //}

            //int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingBacSi.tenBacSi = entity.tenBacSi;
            existingBacSi.bangCap = entity.bangCap;
            
            //existingBacSi.CreateBy = userId;
            

            _context.BacSiEntities.Update(existingBacSi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTkMk(int id, BacSiModel entity)
        {
            //byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            //int userId = BitConverter.ToInt32(userIdBytes, 0);

            var existingBacSi = await _context.BacSiEntities
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingBacSi == null)
            {
                throw new Exception(message: "Account not found!");
            }

            //existingNhanVien.SDT = entity.SDT;
            //existingNhanVien.CMND = entity.CMND;
            existingBacSi.email = entity.email;
            existingBacSi.matKhau = EncryptPassword(entity.matKhau);
            //existingNhanVien.tenNhanVien = entity.tenNhanVien;

            _context.BacSiEntities.Update(existingBacSi);
            await _context.SaveChangesAsync();
        }
        public static string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }
            else
            {
                // Generate a random salt
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

                // Create a new instance of the bcrypt hashing algorithm
                var bcrypt = new Rfc2898DeriveBytes(password, salt, 10000);

                // Generate the hash value
                byte[] hash = bcrypt.GetBytes(20);

                // Combine the salt and hash into a single string
                byte[] storedPassword = new byte[36];
                Array.Copy(salt, 0, storedPassword, 0, 16);
                Array.Copy(hash, 0, storedPassword, 16, 20);

                string encryptedPassword = Convert.ToBase64String(storedPassword);
                return encryptedPassword;
            }
        }
        public async Task UpdateAvatar(string avatarUrl)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var user = _context.BacSiEntities.FirstOrDefault(p => p.Id == userId);
                if (user != null)
                {
                    user.hinhAnh = avatarUrl;
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
