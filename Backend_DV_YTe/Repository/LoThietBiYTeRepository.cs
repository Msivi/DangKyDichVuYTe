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
    public class LoThietBiYTeRepository:ILoThietBiYTeRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public LoThietBiYTeRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        public async Task<List<LoThietBiYTeEntity>> getLoThietBiYTeByMaThietBi(int maThuoc)
        {
            try
            {
                var listLo = _context.loThietBiYTeEntities
                    .Where(c => c.MaThietbiYTe == maThuoc && c.DeletedTime == null)
                    .ToList();
                return listLo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<LoThietBiYTeEntity>> getAllLoThietBiYTe()
        {
            try
            {
                var listLo = _context.loThietBiYTeEntities
                    .Where(c => c.DeletedTime == null)
                    .ToList();
                return listLo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task CheckAndNotifyExpiringLoThietBiYTe()
        {
            var expiringDate = DateTime.Now.AddDays(15);
            var expiringLoThietBiYTe = await GetExpiringLoThietBiYTe(expiringDate);

            if (expiringLoThietBiYTe.Any())
            {
                string emailBody = "Các lô thiết bị y tế sau đây sắp hết hạn:\n\n";
                foreach (var LoThietBiYTe in expiringLoThietBiYTe)
                {
                    emailBody += $"Tên thiết bị: {LoThietBiYTe.ThietBiYTe.tenThietBi}, Ngày hết hạn: {LoThietBiYTe.ngayHetHan.ToShortDateString()}, Số lượng: {LoThietBiYTe.soLuong}\n";
                }

                var emailQL = await GetEmailQL();
                foreach (var email in emailQL)
                {
                    await SendEmailAsync(email, "Thông báo lô thuốc sắp hết hạn", emailBody);
                }
            }
        }

        public async Task<List<LoThietBiYTeEntity>> GetExpiringLoThietBiYTe(DateTime expiringDate)
        {
            return await _context.loThietBiYTeEntities
                .Include(lt => lt.ThietBiYTe)
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
        public async Task HuyLoThietBiYTe(int LoThietBiYTeId, string lyDoHuy)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var LoThietBiYTe = await _context.loThietBiYTeEntities.FindAsync(LoThietBiYTeId);
            if (LoThietBiYTe != null)
            {
                var huyLoThietBiYTe = new HuyLoThietBiYTeEntity
                {
                    LoThietBiId = LoThietBiYTeId,
                    NgayHuy = DateTime.Now,
                    CreateBy = userId,
                    LyDoHuy = lyDoHuy
                };

                _context.huyLoThietBiYTeEntities.Add(huyLoThietBiYTe);
                LoThietBiYTe.DeletedTime = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Lô thuốc không tồn tại");
            }
        }

        public async Task<PhieuHuyLoThietBiYTeModel> TaoPhieuHuyLoThietBiYTe(int LoThietBiYTeId, string lyDoHuy)
        {
            await HuyLoThietBiYTe(LoThietBiYTeId, lyDoHuy);

            var LoThietBiYTe = await _context.loThietBiYTeEntities
                .Include(lt => lt.ThietBiYTe)
                .FirstOrDefaultAsync(lt => lt.Id == LoThietBiYTeId);

            if (LoThietBiYTe == null)
            {
                throw new Exception("Lô thuốc không tồn tại");
            }

            var phieuHuyLoThietBiYTe = new PhieuHuyLoThietBiYTeModel
            {
                NgayHuy = DateTime.Now,

                LyDoHuy = lyDoHuy,
                LoThietBiDetails = new List<PhieuHuyLoThietBiYTeModel.LoThietBiInfo>
            {
                new PhieuHuyLoThietBiYTeModel.LoThietBiInfo
                {
                    TenThietBi = LoThietBiYTe.ThietBiYTe.tenThietBi,
                    NgayHetHan = LoThietBiYTe.ngayHetHan,
                    SoLuong = LoThietBiYTe.soLuong
                }
            }
            };

            return phieuHuyLoThietBiYTe;
        }
    }
}
