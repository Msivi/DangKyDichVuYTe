using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using MimeKit;
using static iTextSharp.text.pdf.events.IndexEvents;
using MailKit.Net.Smtp;
using System.Net.Mail;
using System.Net;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichHenController : ControllerBase
    {
        private readonly ILichHenRepository _lichHenRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly AppDbContext _context;
        public LichHenController(ILichHenRepository LichHenRepository, IMapper mapper, IDistributedCache distributedCache, AppDbContext context)
        {
            _lichHenRepository = LichHenRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _context = context;
        }

        [HttpGet]
        [Route("/api/[controller]/get-lich-hen-by-id")]
        public async Task<ActionResult<LichHenEntity>> GetLichHenById(int id)
        {
            try
            {
                var entity = await _lichHenRepository.GetLichHenById(id);

                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpGet]
        [Route("/api/[controller]/get-lich-hen-by-mabs-tg-tt")]
        public async Task<ActionResult<LichHenEntity>> GetLichHenByMaBacSi(int id, DateTime thoiGianDuKien, string trangThai)
        {
            try
            {
                var entity = await _lichHenRepository.GetLichHenByMaBacSi(id, thoiGianDuKien, trangThai);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpGet]
        [Route("/api/[controller]/get-all-lich-hen")]
        public async Task<ActionResult<ICollection<LichHenEntity>>> GetAllLichHen()
        {
            try
            {
                var entity = await _lichHenRepository.GetAllLichHen();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpGet]
        [Route("/api/[controller]/get-all-lich-hen-online")]
        public async Task<ActionResult<ICollection<LichHenOnlineModel>>> GetAllLichHenOnline()
        {
            try
            {
                var entity = await _lichHenRepository.GetAllLichHenOnline();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpGet]
        [Route("/api/[controller]/get-all-lich-hen-khach-hang")]
        public async Task<ActionResult<ICollection<LichHenEntity>>> GetAllLichHenKhachHang()
        {
            try
            {
                var entity = await _lichHenRepository.GetAllLichHenKhachHang();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpGet]
        [Route("/api/[controller]/get-all-lich-hen-bac-si-chua-kham")]
        public async Task<ActionResult<ICollection<LichHenEntity>>> GetAllLichHenBacSi()
        {
            try
            {
                var entity = await _lichHenRepository.GetAllLichHenBacSi();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpGet]
        [Route("/api/[controller]/get-all-lich-hen-bac-si-da-kham")]
        public async Task<ActionResult<ICollection<LichHenEntity>>> GetAllLichHenBacSiDaKham()
        {
            try
            {
                var entity = await _lichHenRepository.GetAllLichHenBacSiDaKham();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        //[HttpGet]
        //[Route("/api/[controller]/get-all-lich-hen-onine")]
        //public async Task<ActionResult<ICollection<LichHenEntity>>> GetAllLichHenOnline()
        //{
        //    try
        //    {
        //        var entity = await _lichHenRepository.GetAllLichHenOnline();

        //        return Ok(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic result = new BaseResponseModel<string>(
        //           statusCode: StatusCodes.Status500InternalServerError,
        //           code: "Failed!",
        //           message: ex.Message);
        //        return BadRequest(result);
        //    }
        //}

        //[HttpGet]
        //[Route("/api/[controller]/search-lich-hen")]
        //public async Task<ActionResult<ICollection<LichHenEntity>>> SearchLichHen(string searchKey)
        //{
        //    try
        //    {
        //        var LichHenList = await _lichHenRepository.SearchLichHen(searchKey);
        //        if (!LichHenList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(LichHenList);
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic result = new BaseResponseModel<string>(
        //            statusCode: StatusCodes.Status500InternalServerError,
        //            code: "Failed!",
        //            message: ex.Message);
        //        return BadRequest(result);
        //    }
        //}

        //[HttpPost]
        //[Route("/api/[controller]/create-lich-hen")]
        //public async Task<ActionResult<string>> CreateLichHen(LichHenModel entity)
        //{
        //    try
        //    {


        //        var mapEntity = _mapper.Map<LichHenEntity>(entity);


        //        var result = await _lichHenRepository.CreateLichHen(mapEntity);

        //        SendEmail(mapEntity.Id);

        //        return Ok(new BaseResponseModel<string>(
        //            statusCode: StatusCodes.Status201Created,
        //            code: "Success!",
        //            data: result));
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic result;
        //        result = new BaseResponseModel<string>(
        //           statusCode: StatusCodes.Status500InternalServerError,
        //           code: "Failed!",
        //           message: ex.Message);
        //        return BadRequest(result);
        //    }
        //}
        [HttpPost]
        [Route("/api/[controller]/create-lich-hen")]
        public async Task<ActionResult<string>> CreateLichHen(LichHenModel entity)
        {
            try
            {
                var mapEntity = _mapper.Map<LichHenEntity>(entity);
                var result = await _lichHenRepository.CreateLichHen(mapEntity);

                if (string.IsNullOrEmpty(result))
                {
                    // Xử lý lỗi tạo (ghi nhật ký lỗi, trả về thông báo lỗi phù hợp)
                    return BadRequest("Không thể tạo lịch hẹn.");
                }

                //await SendEmail(int.Parse(result)); // Giả sử result là ID lịch hẹn

                return Ok(new BaseResponseModel<string>(
                  statusCode: StatusCodes.Status201Created,
                  code: "Success!",
                  data: result));
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ trong quá trình xử lý
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("/api/[controller]/update-lich-hen")]
        public async Task<IActionResult> UpdateLichHen(int id, LichHenModel entity)
        {
            try
            {


                await _lichHenRepository.UpdateLichHen(id, entity);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Updated successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Error",
                    message: ex.Message));
            }
        }
        [HttpPut]
        [Route("/api/[controller]/update-huy-lich-hen")]
        public async Task<IActionResult> UpdateHuyLichHen(int id)
        {
            try
            {


                await _lichHenRepository.UpdateHuyLichHen(id);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Updated successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Error",
                    message: ex.Message));
            }
        }
        [HttpPut]
        [Route("/api/[controller]/update-lich-hen-nhan-vien")]
        public async Task<IActionResult> UpdateLichHenNhanVien(int id, string diaDiem)
        {
            try
            {
                //byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
                //int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<LichHenEntity>(entity);
                //mapEntity.CreateBy = userId;

                await _lichHenRepository.UpdateLichHenNhanVien(id, diaDiem);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Updated successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Error",
                    message: ex.Message));
            }
        }
        [HttpDelete]
        [Route("/api/[controller]/delete-lich-hen")]
        public async Task<ActionResult<LichHenEntity>> DeleteLichHen(int keyId)
        {

            try
            {

                await _lichHenRepository.DeleteLichHen(keyId, false);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Delete successfully!"));
            }
            catch (Exception ex)
            {

                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Error",
                    message: ex.Message));
            }
        }

        //[HttpPost]
        //[Route("/api/[controller]/send-email")]
        //public async Task<IActionResult> SendEmail(int maLichHen)
        //{
        //    byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
        //    if (userIdBytes == null || userIdBytes.Length != sizeof(int))
        //    {
        //        throw new Exception(message: "Vui lòng đăng nhập!");
        //    }

        //    int userId = BitConverter.ToInt32(userIdBytes, 0);

        //    var existingEmail = await _context.khachHangEntities
        //                                    .Where(c => c.maKhachHang == userId)
        //                                    .Select(c => new {email= c.email,tenKh= c.tenKhachHang })
        //                                    .FirstOrDefaultAsync();

        //    var existingLichHen = await _context.lichHenEntities
        //                            .Include(d => d.DichVu)
        //                            .FirstOrDefaultAsync(c => c.Id == maLichHen);

 
        //    string body = "Xin chào! " + existingEmail.tenKh + "<br>" +
        //                  "Cảm ơn bạn đã tin tưởng chúng tôi.<br>" +
        //                  "Sau đây chúng tôi xin thông báo lịch hẹn khám bệnh mà bạn đã đặt trên hệ  thống của chúng tôi<br>" +
        //                  $"Tên dịch vụ: {existingLichHen.DichVu.tenDichVu}<br>" +
        //                  $"Địa điểm: {existingLichHen.diaDiem}<br>" +
        //                  $"Thời gian: {existingLichHen.thoiGianDuKien}<br>";
                       

        //    var email = new MimeMessage();
        //    email.From.Add(MailboxAddress.Parse("july6267@gmail.com"));
        //    email.To.Add(MailboxAddress.Parse(existingEmail.email));
        //    email.Subject = "Thông tin lịch hẹn";
        //    email.Body = new TextPart(TextFormat.Html) { Text = body };

        //    using (var client = new SmtpClient())
        //    {
        //        try
        //        {
        //            client.Connect("smtp.gmail.com", 587, false);
        //            client.Authenticate("july6267@gmail.com", "tape xhgh khov qusg");

        //            client.Send(email);
        //            client.Disconnect(true);
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //        }
        //    }

        //    return Ok();
        //}


        // test
        //public static bool SendMail(ThanhToanModel datTourModel)
        //{

        //    double TongTien = datTourModel.soLuong * datTourModel.Tour.chiPhi;
        //    string listThanhVien = "";
        //    if (datTourModel.ThanhViens != null)
        //    {
        //        int i = 0;
        //        foreach (var item in datTourModel.ThanhViens)
        //        {
        //            listThanhVien += ++i + ". Họ tên: " + item.hoTen + " - giới tính: " + item.gioiTinh + " - Ngày sinh: " + item.ngaySinh.ToShortDateString() + "\n";

        //        }
        //    }

        //    string email = "vonguyenduytan08022002@gmail.com";
        //    string password = "kfyk nufa clxf sstl";

        //    // Tạo đối tượng MailMessage
        //    MailMessage mail = new MailMessage();
        //    mail.From = new MailAddress(email);
        //    mail.To.Add(datTourModel.KhachHang.email);
        //    mail.Subject = "Thong bao";
        //    mail.Body = "Xin chào!" + datTourModel.KhachHang.hoTen +
        //        "Cảm ơn bạn đã tin tưởng chúng tôi.\n" +
        //        "Sau đây chúng tôi xin thông báo tour mà bạn đã đặt trên hệ  thống của chúng tôi\n" +
        //        "Mã Tour: " + datTourModel.maTour + " - Tên Tour: " + datTourModel.Tour.tenTour
        //        + "\n Giá: " + datTourModel.Tour.chiPhi.ToString("N0") + " VND - so luong: " + datTourModel.soLuong
        //        + "\n Tổng cộng: " + TongTien.ToString("N0") + "VND \n"
        //        + "Thành viên:\n"
        //        + listThanhVien;

        //    // Cấu hình thông tin SMTP server
        //    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
        //    smtpClient.Port = 587;
        //    smtpClient.Credentials = new NetworkCredential(email, password);
        //    smtpClient.EnableSsl = true;

        //    try
        //    {
        //        // Gửi email
        //        smtpClient.Send(mail);

        //        Console.WriteLine("Email sent successfully.");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Failed to send email. Error message: " + ex.Message);
        //        return false;
        //    }
        //}
        //public static bool SendMailForgetPass(TaiKhoanModel TaiKhoanModel_)
        //{

        //    string email = "vonguyenduytan08022002@gmail.com";
        //    string password = "kfyk nufa clxf sstl";

        //    // Tạo đối tượng MailMessage
        //    MailMessage mail = new MailMessage();
        //    mail.From = new MailAddress(email);
        //    mail.To.Add(TaiKhoanModel_.email);
        //    mail.Subject = "Thong bao";
        //    mail.Body = "Xin chào! " + TaiKhoanModel_.email + "\n Mật khẩu mới của bạn là: " + TaiKhoanModel_.matKhau +
        //        "\n Vui lòng không chia sẻ mật khẩu cho bất kì ai";

        //    // Cấu hình thông tin SMTP server
        //    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
        //    smtpClient.Port = 587;
        //    smtpClient.Credentials = new NetworkCredential(email, password);
        //    smtpClient.EnableSsl = true;

        //    try
        //    {
        //        // Gửi email
        //        smtpClient.Send(mail);

        //        Console.WriteLine("Email sent successfully.");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Failed to send email. Error message: " + ex.Message);
        //        return false;
        //    }
        //}
        //public string GetGioiTinh(bool gt)
        //{
        //    return (gt == true) ? "Nam" : "Nữ";
        //}
    }
}

