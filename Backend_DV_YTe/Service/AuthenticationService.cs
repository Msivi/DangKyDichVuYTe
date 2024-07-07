using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Backend_DV_YTe.Service
{
     
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppSettingModel _appSettings;
        private readonly AppDbContext _context;
 
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        public AuthenticationService(  IOptions<AppSettingModel> appSettings, AppDbContext Context, IDistributedCache distributedCache, IMapper mapper)
        {
           
            _appSettings = appSettings.Value;
            _context = Context;
            _distributedCache = distributedCache;
            _mapper = mapper;
        }
        //public string GenerateToken(KhachHangEntity nguoiDung)
        //{
        //    var jwtTokenHandler = new JwtSecurityTokenHandler();
        //    var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

        //    var tokenDescription = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //        new Claim(ClaimTypes.Name, nguoiDung.tenKhachHang),
        //        new Claim(ClaimTypes.Role, nguoiDung.Role),
        //        new Claim(ClaimTypes.Email, nguoiDung.email),
        //        new Claim("Id", nguoiDung.maKhachHang.ToString()),
        //        new Claim("TokenId", Guid.NewGuid().ToString())
        //    }),
        //        Expires = DateTime.UtcNow.AddMinutes(10),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
        //    };

        //    var token = jwtTokenHandler.CreateToken(tokenDescription);

        //    return jwtTokenHandler.WriteToken(token);
        //}

        ////public string GenerateToken(KhachHangEntity nguoiDung)
        ////{
        ////    var jwtTokenHandler = new JwtSecurityTokenHandler();
        ////    var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

        ////    var tokenDescription = new SecurityTokenDescriptor
        ////    {
        ////        Subject = new ClaimsIdentity(new[]
        ////        {
        ////        new Claim(ClaimTypes.Name, nguoiDung.tenKhachHang),
        ////        new Claim(ClaimTypes.Role, nguoiDung.Role),
        ////        new Claim(ClaimTypes.Email, nguoiDung.email),
        ////        new Claim("Id", nguoiDung.maKhachHang.ToString()),
        ////        new Claim("TokenId", Guid.NewGuid().ToString())
        ////    }),
        ////        Expires = DateTime.UtcNow.AddMinutes(10),
        ////        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
        ////    };

        ////    var token = jwtTokenHandler.CreateToken(tokenDescription);

        ////    return jwtTokenHandler.WriteToken(token);
        ////}




        public async Task<string> CreateKhachHang(KhachHangModel entity)
        {
            var mapEntity = _mapper.Map<KhachHangEntity>(entity);

            // Kiểm tra định dạng email
            bool isValidEmail = IsValidEmail(mapEntity.email);
            if (!isValidEmail)
            {
                throw new Exception("Email không đúng định dạng!");
            }

            bool emailExists = await _context.khachHangEntities.AnyAsync(c => c.email == mapEntity.email);
            if (emailExists)
            {
                throw new Exception("Email đã tồn tại!");
            }

            mapEntity.matKhau = EncryptPassword(entity.matKhau);

            await _context.khachHangEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.maKhachHang.ToString();
        }
        public async Task<string> CreateBacSi(BacSiModel entity)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập role quản lý!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var mapEntity = _mapper.Map<BacSiEntity>(entity);

            // Kiểm tra định dạng email
            bool isValidEmail = IsValidEmail(mapEntity.email);
            if (!isValidEmail)
            {
                throw new Exception("Email không đúng định dạng!");
            }

            bool emailExists = await _context.BacSiEntities.AnyAsync(c => c.email == mapEntity.email);
            if (emailExists)
            {
                throw new Exception("Email đã tồn tại!");
            }

            mapEntity.matKhau = EncryptPassword(entity.matKhau);
            mapEntity.CreateBy = userId;
            await _context.BacSiEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email && !email.EndsWith(".");
            }
            catch
            {
                return false;
            }
        }
        //private bool CompareEncryptedPasswords(string password, byte[] storedPassword)
        //{
        //    try
        //    {
        //        var salt = new byte[16];
        //        Array.Copy(storedPassword, 0, salt, 0, 16);

        //        var bcrypt = new Rfc2898DeriveBytes(password, salt, 10000);
        //        var hash = bcrypt.GetBytes(20);

        //        return storedPassword.Skip(16).SequenceEqual(hash);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error comparing passwords: " + ex.Message);
        //    }
        //}

        //public static string EncryptPassword(string password)
        //{
        //    if (string.IsNullOrEmpty(password))
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        // Generate a random salt
        //        byte[] salt = new byte[16];
        //        new RNGCryptoServiceProvider().GetBytes(salt);

        //        // Create a new instance of the bcrypt hashing algorithm
        //        var bcrypt = new Rfc2898DeriveBytes(password, salt, 10000);

        //        // Generate the hash value
        //        byte[] hash = bcrypt.GetBytes(20);

        //        // Combine the salt and hash into a single string
        //        byte[] storedPassword = new byte[36];
        //        Array.Copy(salt, 0, storedPassword, 0, 16);
        //        Array.Copy(hash, 0, storedPassword, 16, 20);

        //        return Convert.ToBase64String(storedPassword);
        //    }
        //}

        //public async Task<dynamic> ChangePassword(ChangePasswordRequest entity)
        //{
        //    try
        //    {
        //        // Lấy giá trị UserId từ Distributed Cache
        //        byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
        //        if (userIdBytes == null || userIdBytes.Length != sizeof(int))
        //        {
        //            throw new Exception("Vui lòng đăng nhập!");
        //        }

        //        int userId = BitConverter.ToInt32(userIdBytes, 0);

        //        // Lấy thông tin người dùng (nhân viên, khách hàng hoặc bác sĩ)
        //        var nhanVien = await _context.nhanVienEntities.FirstOrDefaultAsync(p => p.Id == userId && p.DeletedTime == null);
        //        var khachHang = await _context.khachHangEntities.FirstOrDefaultAsync(p => p.maKhachHang == userId && p.DeletedTime == null);
        //        var bacSi = await _context.BacSiEntities.FirstOrDefaultAsync(p => p.Id == userId && p.DeletedTime == null);

        //        // Xác định đối tượng người dùng
        //        dynamic user = nhanVien ?? (dynamic)khachHang ?? bacSi;
        //        if (user == null)
        //        {
        //            throw new Exception("Change password unsuccessful!");
        //        }

        //        // Kiểm tra mật khẩu hiện tại
        //        var storedPassword = Convert.FromBase64String(user.matKhau);
        //        if (!CompareEncryptedPasswords(entity.CurrentPassword, storedPassword))
        //        {
        //            throw new Exception("Current password is incorrect!");
        //        }

        //        // Mã hóa và lưu mật khẩu mới
        //        var encryptedNewPassword = EncryptPassword(entity.NewPassword);
        //        user.matKhau = encryptedNewPassword;

        //        // Lưu thay đổi vào cơ sở dữ liệu
        //        await _context.SaveChangesAsync();

        //        return new { Success = true, Message = "Password changed successfully!" };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error during password change: " + ex.Message);
        //    }
        //}

        public async Task<dynamic> ChangePassword(ChangePasswordRequest entity)
        {
            try
            {
                // Lấy giá trị UserId từ Distributed Cache
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception("Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                // Lấy thông tin người dùng (nhân viên, khách hàng hoặc bác sĩ)
                var nhanVien = await _context.nhanVienEntities.FirstOrDefaultAsync(p => p.Id == userId && p.DeletedTime == null);
                var khachHang = await _context.khachHangEntities.FirstOrDefaultAsync(p => p.maKhachHang == userId && p.DeletedTime == null);
                var bacSi = await _context.BacSiEntities.FirstOrDefaultAsync(p => p.Id == userId && p.DeletedTime == null);

                // Xác định đối tượng người dùng
                dynamic user = nhanVien ?? (dynamic)khachHang ?? bacSi;
                if (user == null)
                {
                    throw new Exception("Change password unsuccessful!");
                }

                // Kiểm tra mật khẩu hiện tại
                if (!CompareEncryptedPasswords(entity.CurrentPassword, user.matKhau))
                {
                    throw new Exception("Current password is incorrect!");
                }

                // Mã hóa và lưu mật khẩu mới
                var encryptedNewPassword = EncryptPassword(entity.NewPassword);
                user.matKhau = encryptedNewPassword;

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return new { Success = true, Message = "Password changed successfully!" };
            }
            catch (Exception ex)
            {
                throw new Exception("Error during password change: " + ex.Message);
            }
        }

        private bool CompareEncryptedPasswords(string password, string storedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, storedPassword);
            }
            catch (Exception ex)
            {
                throw new Exception("Error comparing passwords: " + ex.Message);
            }
        }

        public static string EncryptPassword(string password)
        {
            try
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
            catch (Exception ex)
            {
                throw new Exception("Error encrypting password: " + ex.Message);
            }
        }

        public string GenerateToken(KhachHangEntity nguoiDung)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, nguoiDung.tenKhachHang),
                    new Claim(ClaimTypes.Role, nguoiDung.Role),
                    new Claim(ClaimTypes.Email, nguoiDung.email),
                    new Claim("Id", nguoiDung.maKhachHang.ToString()),
                    new Claim("TokenId", Guid.NewGuid().ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }
        public string GenerateToken(BacSiEntity bacSi)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, bacSi.tenBacSi),
                    new Claim(ClaimTypes.Role, bacSi.Role),
                    new Claim(ClaimTypes.Email, bacSi.email),
                    new Claim("Id", bacSi.Id.ToString()),
                    new Claim("TokenId", Guid.NewGuid().ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }
        public string GenerateToken(NhanVienEntity nguoiDung)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, nguoiDung.tenNhanVien),
                    new Claim(ClaimTypes.Role, nguoiDung.Role),
                    new Claim(ClaimTypes.Email, nguoiDung.email),
                    new Claim("Id", nguoiDung.Id.ToString()),
                    new Claim("TokenId", Guid.NewGuid().ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }

        //public async Task<dynamic> Login(LoginModel entity)
        //{
        //    try
        //    {
        //        var khachHang = await _context.khachHangEntities.SingleOrDefaultAsync(p => p.email == entity.Email);
        //        var nhanVien = await _context.nhanVienEntities.SingleOrDefaultAsync(p => p.email == entity.Email);
        //        var bacsi = await _context.BacSiEntities.SingleOrDefaultAsync(p => p.email == entity.Email);
        //        if (khachHang != null)
        //        {
        //            var encryptedPassword = EncryptPassword(entity.Password);
        //            var storedPassword = Convert.FromBase64String(khachHang.matKhau);

        //            if (!CompareEncryptedPasswords(entity.Password, storedPassword))
        //            {
        //                throw new Exception("Login unsuccessful!");
        //            }

        //            return khachHang;
        //        }
        //        else if (nhanVien !=null)
        //        {
        //            var encryptedPassword = EncryptPassword(entity.Password);
        //            var storedPassword = Convert.FromBase64String(nhanVien.matKhau);

        //            if (!CompareEncryptedPasswords(entity.Password, storedPassword))
        //            {
        //                throw new Exception("Login unsuccessful!");
        //            }

        //            return nhanVien;
        //        }
        //        else if (bacsi != null)
        //        {
        //            var encryptedPassword = EncryptPassword(entity.Password);
        //            var storedPassword = Convert.FromBase64String(bacsi.matKhau);

        //            if (!CompareEncryptedPasswords(entity.Password, storedPassword))
        //            {
        //                throw new Exception("Login unsuccessful!");
        //            }

        //            return bacsi;
        //        }

        //        else
        //        {
        //            throw new Exception("Login unsuccessful!");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error during login: " + ex.Message);
        //    }
        //}
        public async Task<dynamic> Login(LoginModel entity)
        {
            try
            {
                // Tìm người dùng theo email
                var khachHang = await _context.khachHangEntities.SingleOrDefaultAsync(p => p.email == entity.Email && p.DeletedTime == null); 
                var nhanVien = await _context.nhanVienEntities.SingleOrDefaultAsync(p => p.email == entity.Email && p.DeletedTime == null);
                var bacSi = await _context.BacSiEntities.SingleOrDefaultAsync(p => p.email == entity.Email && p.DeletedTime == null);

                if (khachHang != null)
                {
                    if (!CompareEncryptedPasswords(entity.Password, khachHang.matKhau))
                    {
                        throw new Exception("Login unsuccessful!");
                    }

                    return khachHang;
                }
                else if (nhanVien != null)
                {
                    if (!CompareEncryptedPasswords(entity.Password, nhanVien.matKhau))
                    {
                        throw new Exception("Login unsuccessful!");
                    }

                    return nhanVien;
                }
                else if (bacSi != null)
                {
                    if (!CompareEncryptedPasswords(entity.Password, bacSi.matKhau))
                    {
                        throw new Exception("Login unsuccessful!");
                    }

                    return bacSi;
                }
                else
                {
                    throw new Exception("Login unsuccessful!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message);
            }
        }

        public async Task<string> CreateNhanVien(NhanVienModel entity)
        {
            var existingNhanVien = await _context.nhanVienEntities
                .FirstOrDefaultAsync(c => c.email == entity.email && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingNhanVien != null)
            {
                throw new Exception(message: "Account name is already exist!");
            }

            var mapEntity = _mapper.Map<NhanVienEntity>(entity);
            mapEntity.matKhau = EncryptPassword(entity.matKhau);
            mapEntity.Role = "NhanVien";

            await _context.nhanVienEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.email;
        }

        public async Task<string> CreateQuanLy(NhanVienModel entity)
        {
            var existingNhanVien = await _context.nhanVienEntities
                .FirstOrDefaultAsync(c => c.email == entity.email && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingNhanVien != null)
            {
                throw new Exception(message: "Account name is already exist!");
            }

            var mapEntity = _mapper.Map<NhanVienEntity>(entity);
            mapEntity.matKhau = EncryptPassword(entity.matKhau);
            mapEntity.Role = "QuanLy";

            await _context.nhanVienEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.email;
        }
    }
}
