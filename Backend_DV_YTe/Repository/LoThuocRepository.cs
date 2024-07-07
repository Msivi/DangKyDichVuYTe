using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MimeKit;

namespace Backend_DV_YTe.Repository
{
    public class LoThuocRepository : ILoThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public LoThuocRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        
        public async Task<List<LoThuocEntity>> getLoThuocByMaThuoc(int maThuoc)
        {
            try
            {
                var listLo=_context.loThuocEntities
                    .Where(c=>c.MaThuoc==maThuoc&& c.DeletedTime==null)
                    .ToList();
                return listLo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<LoThuocEntity>> getAllLoThuoc()
        {
            try
            {
                var listLo = _context.loThuocEntities
                    .Where(c=>c.DeletedTime == null)
                    .ToList();
                return listLo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task CheckAndNotifyExpiringLoThuoc()
        {
            var expiringDate = DateTime.Now.AddDays(15);
            var expiringLoThuoc = await GetExpiringLoThuoc(expiringDate);

            if (expiringLoThuoc.Any())
            {
                string emailBody = "Các lô thuốc sau đây sắp hết hạn:\n\n";
                foreach (var loThuoc in expiringLoThuoc)
                {
                    emailBody += $"Tên thuốc: {loThuoc.Thuoc.tenThuoc}, Ngày hết hạn: {loThuoc.ngayHetHan.ToShortDateString()}, Số lượng: {loThuoc.soLuong}\n";
                }

                var emailQL = await GetEmailQL();
                foreach (var email in emailQL)
                {
                    await SendEmailAsync(email, "Thông báo lô thuốc sắp hết hạn", emailBody);
                }
            }
        }

        public async Task<List<LoThuocEntity>> GetExpiringLoThuoc(DateTime expiringDate)
        {
            return await _context.loThuocEntities
                .Include(lt => lt.Thuoc)
                .Where(lt => lt.ngayHetHan <= expiringDate && lt.DeletedTime == null)
                .ToListAsync();
        }

        public async Task<List<string>> GetEmailQL()
        {
            var emailQL = await _context.nhanVienEntities
                .Where(c => c.DeletedTime == null && c.Role == "QuanLy")
                .Select(c => c.email)
                .ToListAsync();

            return emailQL;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Your Name", "july6267@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("july6267@gmail.com", "tape xhgh khov qusg");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Gửi email không thành công: " + ex.Message);
                }
            }


        }
        public async Task HuyLoThuoc(int loThuocId, string lyDoHuy)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var loThuoc = await _context.loThuocEntities.FindAsync(loThuocId);
            if (loThuoc != null)
            {
                var huyLoThuoc = new HuyLoThuocEntity
                {
                    LoThuocId = loThuocId,
                    NgayHuy = DateTime.Now,
                    CreateBy = userId,
                    LyDoHuy = lyDoHuy
                };

                _context.huyLoThuocEntities.Add(huyLoThuoc);
                loThuoc.DeletedTime = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Lô thuốc không tồn tại");
            }
        }

        public async Task<PhieuHuyLoThuocModel> TaoPhieuHuyLoThuoc(int loThuocId, string lyDoHuy)
        {
            await HuyLoThuoc(loThuocId, lyDoHuy);

            var loThuoc = await _context.loThuocEntities
                .Include(lt => lt.Thuoc)
                .FirstOrDefaultAsync(lt => lt.Id == loThuocId);

            if (loThuoc == null)
            {
                throw new Exception("Lô thuốc không tồn tại");
            }

            var phieuHuyLoThuoc = new PhieuHuyLoThuocModel
            {
                NgayHuy = DateTime.Now,
               
                LyDoHuy = lyDoHuy,
                LoThuocDetails = new List<PhieuHuyLoThuocModel.LoThuocInfo>
            {
                new PhieuHuyLoThuocModel.LoThuocInfo
                {
                    TenThuoc = loThuoc.Thuoc.tenThuoc,
                    NgayHetHan = loThuoc.ngayHetHan,
                    SoLuong = loThuoc.soLuong
                }
            }
            };

            return phieuHuyLoThuoc;
        }
    }
}
