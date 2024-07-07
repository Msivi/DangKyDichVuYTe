using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Backend_DV_YTe.Repository
{
    public class LichHenRepository:ILichHenRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public LichHenRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        public async Task<string> CreateLichHen(LichHenEntity entity)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var existingLichBS = await _context.lichHenEntities.Where(c => c.MaBacSi == entity.MaBacSi && c.thoiGianDuKien == entity.thoiGianDuKien && c.DeletedTime == null).ToListAsync();
                if (existingLichBS.Any())
                {
                    throw new Exception("Bác sĩ này đã có ca làm. Vui lòng chọn thời gian khác!");
                }

                var existingBS = await _context.lichHenEntities.Where(c => c.MaBacSi == entity.MaBacSi && c.DeletedTime == null).ToListAsync();
                if(existingBS is null)
                {
                    throw new Exception(message:"Mã bác sĩ không được để trống!");
                }
               

                var existingLoaiThietBi = await _context.lichHenEntities
                   .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

                if (existingLoaiThietBi != null)
                {
                    throw new Exception(message: "Id is already exist!");
                }
                var loaiDV = await _context.dichVuEntities.FirstOrDefaultAsync(c => c.Id == entity.MaDichVu);
                if (loaiDV.MaLoaiDichVu != 1)
                {
                    entity.diaDiem = "Link online sẽ được cập nhật khi đến thời gian khám";
                }
                var mapEntity = _mapper.Map<LichHenEntity>(entity);
                mapEntity.MaKhachHang = userId;
                mapEntity.trangThai = "Chưa khám";

                await _context.lichHenEntities.AddAsync(mapEntity);
                await _context.SaveChangesAsync();

                return mapEntity.Id.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<LichHenEntity> DeleteLichHen(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.lichHenEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.lichHenEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.lichHenEntities.Update(entity);
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

        public async Task<ICollection<LichHenEntity>> GetAllLichHen()
        {
            try
            {
                var entities = await _context.lichHenEntities
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
        public async Task<ICollection<LichHenEntity>> GetAllLichHenKhachHang()
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var entities = await _context.lichHenEntities
                     .Where(c =>c.MaKhachHang==userId && c.DeletedTime == null)

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
        public async Task<ICollection<LichHenEntity>> GetAllLichHenBacSi()
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var entities = await _context.lichHenEntities
                     .Where(c => c.MaBacSi == userId &&c.trangThai== "Chưa khám" && c.DeletedTime == null)
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
        public async Task<ICollection<LichHenEntity>> GetAllLichHenBacSiDaKham()
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var entities = await _context.lichHenEntities
                     .Where(c => c.MaBacSi == userId && c.trangThai == "Đã khám" && c.DeletedTime == null)
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

        public async Task<ICollection<LichHenOnlineModel>> GetAllLichHenOnline()
        {
            try
            {
                var query = from lichHen in _context.lichHenEntities
                            join dichVu in _context.dichVuEntities on lichHen.MaDichVu equals dichVu.Id
                            join loaiDV in _context.loaiDichVuEntities on dichVu.MaLoaiDichVu equals loaiDV.Id
                            where loaiDV.Id != 1
                            select new LichHenOnlineModel
                            {
                                id = lichHen.Id,
                                trangThai = lichHen.trangThai,
                               
                                diaDiem = lichHen.diaDiem,
                                ghiChu = lichHen.ghiChu,
                                MaBacSi = lichHen.MaBacSi,
                                MaDichVu = lichHen.MaDichVu,
                                tenDichVu = dichVu.tenDichVu,
                                thoiGianDuKien = lichHen.thoiGianDuKien
                            };


                var entities = await query.ToListAsync();
                                    

                if (entities == null || !entities.Any())
                {
                    throw new Exception("Empty list!");
                }

                return entities;
            }
            catch (Exception ex)
            {
                // Consider logging the exception here using a logging framework
                throw new Exception("An error occurred while fetching the data: " + ex.Message, ex);
            }
        }



        public async Task<LichHenEntity> GetLichHenById(int id)
        {
            var entity = await _context.lichHenEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }
        public async Task<LichHenEntity> GetLichHenByMaBacSi(int id, DateTime ngayDK, string trangThai)
        {
            var entity = await _context.lichHenEntities.FirstOrDefaultAsync(e => e.Id == id && e.thoiGianDuKien==ngayDK &&e.trangThai==trangThai && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }


        public async Task UpdateHuyLichHen(int id)
        {

            var existingLichHen = await _context.lichHenEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingLichHen == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            if(existingLichHen.trangThai== "Đã khám")
            {
                throw new Exception(message: "Dịch vụ này quý khách đã sử dụng rồi không thể huy lịch!");
            }    
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingLichHen.trangThai = "Đã hủy";

            existingLichHen.MaKhachHang = userId;


            _context.lichHenEntities.Update(existingLichHen);
            await _context.SaveChangesAsync();
        }
        //public async Task<ICollection<LichHenEntity>> SearchLichHen(string searchKey)
        //{
        //    var ListKH = await _context.lichHenEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (

        //        c.tenLichHen.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.chuyenKhoa.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.bangCap.Contains(searchKey, StringComparison.OrdinalIgnoreCase)

        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

         
        public async Task UpdateLichHen(int id, LichHenModel entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingLichHen = await _context.lichHenEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingLichHen == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingLichHen.diaDiem = entity.diaDiem;
            existingLichHen.thoiGianDuKien = entity.thoiGianDuKien;
            existingLichHen.MaBacSi = entity.MaBacSi;
            existingLichHen.MaDichVu = entity.MaDichVu;
            existingLichHen.ghiChu= entity.ghiChu;
            existingLichHen.MaKhachHang = userId;


            _context.lichHenEntities.Update(existingLichHen);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLichHenNhanVien(int id, string diaDiem)
        {

            var existingLichHen = await _context.lichHenEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingLichHen == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            existingLichHen.diaDiem = diaDiem;
            existingLichHen.CreateBy = userId;


            _context.lichHenEntities.Update(existingLichHen);
            await _context.SaveChangesAsync();
            // Gọi hàm gửi email sau khi cập nhật thành công
            await SendEmailAsync(existingLichHen);
        }
        public async Task SendEmailAsync(LichHenEntity lichHen)
        {
            // Lấy thông tin khách hàng
            var customer = await _context.khachHangEntities
                .FirstOrDefaultAsync(c => c.maKhachHang == lichHen.MaKhachHang);

            if (customer == null)
            {
                throw new Exception("Không tìm thấy thông tin khách hàng.");
            }

            // Tạo nội dung email
            string body = $"Xin chào {customer.tenKhachHang},<br>" +
                          "Lịch hẹn của bạn đã được cập nhật địa điểm.<br>" +
                          $"Địa điểm là: {lichHen.diaDiem}<br>" +
                          $"Thời gian: {lichHen.thoiGianDuKien}<br>";

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("july6267@gmail.com"));
            email.To.Add(MailboxAddress.Parse(customer.email));
            email.Subject = "Cập nhật địa điểm lịch hẹn";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("july6267@gmail.com", "tape xhgh khov qusg");
                    client.Send(email);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Gửi email không thành công: " + ex.Message);
                }
            }
        }


    }
}
