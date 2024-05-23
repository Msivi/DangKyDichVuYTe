using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
 

namespace Backend_DV_YTe.Repository
{
    public class LoaiThuocRepository:ILoaiThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public LoaiThuocRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateLoaiThuoc(LoaiThuocEntity entity)
        {
            var existingLoaiThuoc = await _context.loaiThuocEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingLoaiThuoc != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<LoaiThuocEntity>(entity);

            await _context.loaiThuocEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }
        //============================================
        public void SendEmail(string recipientEmail, string subject, string message)
        {
            // Địa chỉ email người gửi
            string senderEmail = "july6267@gmail.com";
            // Mật khẩu email người gửi
            string senderPassword = "01644440794";

            // Tạo đối tượng MimeMessage
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Sender Name", senderEmail));
            emailMessage.To.Add(new MailboxAddress("", recipientEmail));
            emailMessage.Subject = subject;

            // Tạo nội dung email dạng plain text
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = message;
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // Cấu hình thông tin máy chủ SMTP
            var smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.example.com", 587, false);
            smtpClient.Authenticate(senderEmail, senderPassword);

            // Gửi email
            smtpClient.Send(emailMessage);
            smtpClient.Disconnect(true);
        }
        //============================================


        public async Task<LoaiThuocEntity> DeleteLoaiThuoc(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.loaiThuocEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy nhà cung cấp!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.loaiThuocEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.loaiThuocEntities.Update(entity);
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

        public async Task<ICollection<LoaiThuocEntity>> GetAllLoaiThuoc()
        {
            try
            {
                var entities = await _context.loaiThuocEntities
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

        public async Task<LoaiThuocEntity> GetLoaiThuocById(int id)
        {
            var entity = await _context.loaiThuocEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<LoaiThuocEntity>> SearchLoaiThuoc(string searchKey)
        //{
        //    var ListKH = await _context.loaiThuocEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c..Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SDT.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}

        public async Task UpdateLoaiThuoc(int id, LoaiThuocEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingLoaiThuoc = await _context.loaiThuocEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingLoaiThuoc == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingLoaiThuoc.tenLoaiThuoc = entity.tenLoaiThuoc;
            
            _context.loaiThuocEntities.Update(existingLoaiThuoc);
            await _context.SaveChangesAsync();
        }
    }
}
