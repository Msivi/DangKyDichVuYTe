using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Model.SanPhamHoaDon;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Mail;
using System.Net;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Backend_DV_YTe.Repository
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public HoaDonRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        public async Task<string> CreateHoaDon()
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId"); // Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);


            var newHoaDon = new HoaDonEntity
            {
                ngayMua = DateTime.Now,
                tongTien = 0,
                trangThai = "False",
                MaKhachHang = userId
            };

            await _context.hoaDonEntities.AddAsync(newHoaDon);
            await _context.SaveChangesAsync();

            return newHoaDon.Id.ToString();
        }
        //public async Task UpdateHoaDon(int id, HoaDonEntity entity)
        //{
        //    try
        //    {
        //        if (entity == null)
        //        {
        //            throw new Exception(message: "The entity field is required!");
        //        }

        //        var existingHoaDon = await _context.hoaDonEntities
        //            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

        //        if (existingHoaDon == null)
        //        {
        //            throw new Exception(message: "Không tìm thấy!");
        //        }
        //        byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
        //        if (userIdBytes == null || userIdBytes.Length != sizeof(int))
        //        {
        //            throw new Exception(message: "Vui lòng đăng nhập!");
        //        }

        //        existingHoaDon.ngayMua = DateTime.Now;
        //        existingHoaDon.trangThai=entity.trangThai;


        //        //existingHoaDon.MaKhachHang = userId;


        //        _context.hoaDonEntities.Update(existingHoaDon);
        //        await _context.SaveChangesAsync();
        //    }catch(Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }


        //}
        public async Task UpdateHoaDon(HoaDonEntity hoaDon)
        {
            try
            {
                //using (var connection = new SqlConnection("server=LEVI\\SQLEXPRESS;database=DV_YTe3;uid=sa;pwd=123;TrustServerCertificate=True"))
                //{
                //    await connection.OpenAsync();

                //    using (var command = new SqlCommand())
                //    {
                //        command.Connection = connection;
                //        command.CommandText = "UPDATE HoaDon SET ngayMua = @ngayMua, trangThai = @trangThai, diaChi=@diaChi,ghiChu=@ghiChu WHERE Id = @Id";
                //        command.Parameters.AddWithValue("@Id", hoaDon.Id);
                //        command.Parameters.AddWithValue("@ngayMua", hoaDon.ngayMua);
                //        command.Parameters.AddWithValue("@trangThai", hoaDon.trangThai);
                //        command.Parameters.AddWithValue("@diaChi", hoaDon.diaChi);
                //        command.Parameters.AddWithValue("@ghiChu", hoaDon.ghiChu);
                //        await command.ExecuteNonQueryAsync();
                //    }
                //}
 
                
                var existingHoaDon = await _context.hoaDonEntities
                    .FirstOrDefaultAsync(c => c.Id == hoaDon.Id && c.DeletedTime == null);

                if (existingHoaDon == null)
                {
                    throw new Exception(message: "Không tìm thấy!");
                }
                existingHoaDon.ngayMua = hoaDon.ngayMua;
                existingHoaDon.trangThai= hoaDon.trangThai;
                existingHoaDon.diaChi = hoaDon.diaChi;
                existingHoaDon.ghiChu = hoaDon.ghiChu;
                _context.hoaDonEntities.Update(existingHoaDon);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<HoaDonEntity> DeleteHoaDon(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.hoaDonEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.hoaDonEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.hoaDonEntities.Update(entity);
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

        public async Task<ICollection<HoaDonEntity>> GetAllHoaDon()
        {
            try
            {
                var entities = await _context.hoaDonEntities
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
        //==========================danh sach san pham của mã hóa đơn
        public async Task<ICollection<SanPhamModel>> GetAllSanPhamByMaHD(int maHD)
        {
            try
            {
                var entitiesThuoc = await _context.cTMuaThuocEntities
                    .Include(c => c.Thuoc)
                    .Where(c => c.DeletedTime == null && c.MaHoaDon == maHD)
                    .ToListAsync();

                var entitiesThietBi = await _context.cTMuaThietBiYTeEntities
                    .Include(c => c.ThietBiYTe)
                    .Where(c => c.DeletedTime == null && c.MaHoaDon == maHD)
                    .ToListAsync();

                var combinedEntities = new List<SanPhamModel>();

                combinedEntities.AddRange(entitiesThuoc.Select(MapToThuocMuaModel));
                combinedEntities.AddRange(entitiesThietBi.Select(MapToThietBiMuaModel));

                return combinedEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public thuocMuaModel MapToThuocMuaModel(CTMuaThuocEntity thuocEntity)
        {
            return new thuocMuaModel
            {
                maSanPham = thuocEntity.MaThuoc,
                tenSanPham = thuocEntity.Thuoc.tenThuoc,
                soLuong = thuocEntity.soLuong,
                donGia = thuocEntity.donGia,
                thanhTien = thuocEntity.thanhTien,
                hinhAnh = thuocEntity.Thuoc.hinhAnh,
                maThuoc = thuocEntity.MaThuoc
            };
        }

        public thiBiMuaModel MapToThietBiMuaModel(CTMuaThietBiYTeEntity thietBiEntity)
        {
            return new thiBiMuaModel
            {
                maSanPham = thietBiEntity.MaThietBiYTe,
                tenSanPham = thietBiEntity.ThietBiYTe.tenThietBi,
                soLuong = thietBiEntity.soLuong,
                donGia = thietBiEntity.donGia,
                thanhTien = thietBiEntity.thanhTien,
                hinhAnh = thietBiEntity.ThietBiYTe.hinhAnh,
                maThietBi = thietBiEntity.MaThietBiYTe
            };
        }
        //===================
        //public async Task<HoaDonEntity> GetHoaDonById(int id)
        //{
        //    try
        //    {
        //        var entity = await _context.hoaDonEntities
        //             .Where(c => c.Id == id && c.DeletedTime == null)
        //             .FirstOrDefaultAsync();

        //        if (entity is null)
        //        {
        //            throw new Exception("Empty list!");
        //        }

        //        return entity;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        public async Task<HoaDonEntity> GetHoaDonById(int id)
        {
            try
            {
                //using (var connection = new SqlConnection("server=LEVI\\SQLEXPRESS;database=DV_YTe3;uid=sa;pwd=123;TrustServerCertificate=True"))
                //{
                //    await connection.OpenAsync();

                //    using (var command = new SqlCommand())
                //    {
                //        command.Connection = connection;
                //        command.CommandText = "SELECT * FROM HoaDon WHERE Id = @Id AND DeletedTime IS NULL";
                //        command.Parameters.AddWithValue("@Id", id);

                //        using (var reader = await command.ExecuteReaderAsync())
                //        {
                //            if (await reader.ReadAsync())
                //            {
                //                // Đọc dữ liệu từ DataReader
                //                var entity = new HoaDonEntity();
                //                entity.Id = (int)reader["Id"];
                //                entity.trangThai = (string)reader["trangThai"];
                //                entity.tongTien = (double)reader["tongTien"];

                //                // Trả về đối tượng HoaDonEntity
                //                return entity;
                //            }
                //            else
                //            {
                //                throw new Exception("Empty list!");
                //            }
                //        }
                //    }
                //}
                var entity = await _context.hoaDonEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity is null)
                {
                    throw new Exception("not found or already deleted.");
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ICollection<HoaDonEntity>> GetAllHoaDonKhachHang()
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }
                int userId = BitConverter.ToInt32(userIdBytes, 0);
                var entities = await _context.hoaDonEntities
                     .Where(c => c.MaKhachHang == userId && c.DeletedTime == null)
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
        public ICollection<HoaDonEntity> GetHoaDonByMaKhachHang(int userId)
        {

            var hoaDonList = _context.hoaDonEntities
                .Include(h => h.CTMuaThuoc)
                .Include(h => h.CTMuaThietBiYTe)
                .Where(h => h.MaKhachHang == userId)
                .ToList();

            return hoaDonList;
        }
        
    }
}
