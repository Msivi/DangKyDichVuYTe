﻿using AutoMapper;
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

        private bool CompareEncryptedPasswords(string password, byte[] storedPassword)
        {
            var salt = new byte[16];
            Array.Copy(storedPassword, 0, salt, 0, 16);

            var bcrypt = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = bcrypt.GetBytes(20);

            return storedPassword.Skip(16).SequenceEqual(hash);
        }
        public async Task<dynamic> ChangePassword(ChangePasswordRequest entity)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var nhanVien = await _context.nhanVienEntities.SingleOrDefaultAsync(p => p.Id == userId);
                var khachHang = await _context.khachHangEntities.SingleOrDefaultAsync(p => p.maKhachHang == userId);

                if (nhanVien is not null)
                {
                    var encryptedPassword = EncryptPassword(entity.CurrentPassword);
                    var storedPassword = Convert.FromBase64String(nhanVien.matKhau);

                    if (!CompareEncryptedPasswords(entity.CurrentPassword, storedPassword))
                    {
                        throw new Exception(message: "Current password is incorrect!");
                    }

                    var encryptedNewPassword = EncryptPassword(entity.NewPassword);
                    nhanVien.matKhau = encryptedNewPassword;

                    return await _context.SaveChangesAsync();

                }
                
                else if (khachHang is not null)
                {
                    var encryptedPassword = EncryptPassword(entity.CurrentPassword);
                    var storedPassword = Convert.FromBase64String(khachHang.matKhau);

                    if (!CompareEncryptedPasswords(entity.CurrentPassword, storedPassword))
                    {
                        throw new Exception("Current password is incorrect");
                    }


                    var encryptedNewPassword = EncryptPassword(entity.NewPassword);
                    khachHang.matKhau = encryptedNewPassword;

                    return await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Change password unsuccessful!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during password: " + ex.Message);
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

        public async Task<dynamic> Login(LoginModel entity)
        {
            try
            {
                var khachHang = await _context.khachHangEntities.SingleOrDefaultAsync(p => p.email == entity.Email);
                var nhanVien = await _context.nhanVienEntities.SingleOrDefaultAsync(p => p.email == entity.Email);
                if (khachHang != null)
                {
                    var encryptedPassword = EncryptPassword(entity.Password);
                    var storedPassword = Convert.FromBase64String(khachHang.matKhau);

                    if (!CompareEncryptedPasswords(entity.Password, storedPassword))
                    {
                        throw new Exception("Login unsuccessful!");
                    }

                    return khachHang;
                }
                else if (nhanVien !=null)
                {
                    var encryptedPassword = EncryptPassword(entity.Password);
                    var storedPassword = Convert.FromBase64String(nhanVien.matKhau);

                    if (!CompareEncryptedPasswords(entity.Password, storedPassword))
                    {
                        throw new Exception("Login unsuccessful!");
                    }

                    return nhanVien;
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