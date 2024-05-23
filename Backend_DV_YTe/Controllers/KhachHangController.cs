using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Backend_DV_YTe.Model;
//using Backend_DV_YTe.Entity;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _distributedCache;
        public KhachHangController(IKhachHangRepository khachHangRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment, AppDbContext context, IDistributedCache distributedCache)
        {
            _khachHangRepository = khachHangRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _distributedCache = distributedCache;
        }
        [HttpGet]
        [Route("/api/[controller]/get-khach-hang-by-id")]
        public async Task<ActionResult<KhachHangEntity>> GetKhachHangById(int id)
        {
            try
            {
                var entity = await _khachHangRepository.GetKhachHangById(id);

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
        [Authorize(Roles = "QuanLy")]
        [HttpGet]
        [Route("/api/[controller]/get-all-khach-hang")]
        public async Task<ActionResult<IEnumerable<KhachHangEntity>>> GetAllKhachHang()
        {
            try
            {
                var entity = await _khachHangRepository.GetAllKhachHang();

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
        [Route("/api/[controller]/get-tt-khach-hang")]
        public async Task<ActionResult<IEnumerable<KhachHangEntity>>> GetTTKhachHang()
        {
            try
            {
                var entity = await _khachHangRepository.GetTTKhachHang();

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

        [Authorize(Roles = "QuanLy")]
        [HttpGet]
        [Route("/api/[controller]/search-khach-hang")]
        public async Task<ActionResult<IEnumerable<KhachHangEntity>>> SearchKhachHang(string searchKey)
        {
            try
            {
                var khachHangList = await _khachHangRepository.SearchKhachHang(searchKey);
                if (!khachHangList.Any())
                {
                    return BadRequest("Not found!");
                }
                return Ok(khachHangList);
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


        
        [HttpPost("UploadAvatar")]
        public async Task<IActionResult> UploadDescuento(IFormFile avatarFile)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);


                var user = await _khachHangRepository.GetKhachHangById(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Người dùng không tồn tại." });
                }

                if (avatarFile == null || avatarFile.Length == 0)
                {
                    return BadRequest(new { message = "Vui lòng chọn tệp ảnh." });
                }
                var maxFileSizeInBytes = 5 * 1024 * 1024; // Giới hạn kích thước tệp ảnh (5MB)
                if (avatarFile.Length > maxFileSizeInBytes)
                {
                    return BadRequest("Kích thước tệp ảnh vượt quá giới hạn cho phép.");
                }
                // Kiểm tra loại tệp ảnh
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Loại tệp ảnh không được hỗ trợ. Vui lòng chọn tệp ảnh có định dạng JPG, JPEG, PNG, hoặc GIF.");
                }

                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(avatarFile.FileName);
                var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(fileStream);
                }
                // Xóa ảnh đại diện cũ (nếu có)
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.ContentRootPath, user.Avatar.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                user.Avatar = $"/Images/" + uniqueFileName;
                await _khachHangRepository.UpdateAvatar(user.Avatar);

                return Ok(new { message = "Đường dẫn ảnh đại diện đã được cập nhật." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi cập nhật đường dẫn ảnh đại diện: " + ex.Message });
            }
        }

        
        [HttpGet]
        [Route("api/DownloadPdfFile")]
        public async Task<IActionResult> DownloadPdfFile()
        {
            try
            {
                var entities = await _context.khachHangEntities.ToListAsync();

                if (entities == null)
                {
                    return NotFound();
                }

                BaseFont baseFont = BaseFont.CreateFont("D:\\DoAnKySu\\fontVN\\vps-thanh-hoa.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                using (var document = new iTextSharp.text.Document())
                {
                    var memoryStream = new MemoryStream();
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Tạo bảng
                    PdfPTable table = new PdfPTable(4); // 4 cột trong bảng

                    // Thiết lập font cho tiêu đề cột
                    Font columnHeaderFont = new Font(baseFont, 12, Font.BOLD);

                    // Thêm tiêu đề cột vào bảng
                    table.AddCell(new PdfPCell(new Phrase("Tên khách hàng", columnHeaderFont)));
                    table.AddCell(new PdfPCell(new Phrase("CMND", columnHeaderFont)));
                    table.AddCell(new PdfPCell(new Phrase("SDT", columnHeaderFont)));
                    table.AddCell(new PdfPCell(new Phrase("Email", columnHeaderFont)));

                    // Thêm dữ liệu từ danh sách entities vào bảng
                    foreach (var entity in entities)
                    {
                        table.AddCell(entity.tenKhachHang);
                        table.AddCell(entity.CMND);
                        table.AddCell(entity.SDT);
                        table.AddCell(entity.email);
                    }

                    // Thêm bảng vào tệp tin PDF
                    document.Add(table);

                    document.Close();

                    // Chuyển đổi MemoryStream thành mảng byte
                    byte[] bytes = memoryStream.ToArray();

                    // Trả về tệp tin PDF
                    return File(bytes, "application/pdf", "DanhSachKhachHang.pdf");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //[HttpGet]
        //[Route("api/DownloadPdfFile/{id}")]
        //public async Task<IActionResult> SendPdfFile(int id)
        //{
        //    try
        //    {
        //        var entities = await _khachHangRepository.GetKhachHangById(id);

        //        if (entities == null)
        //        {
        //            return NotFound();
        //        }

        //        BaseFont baseFont = BaseFont.CreateFont("D:\\DoAnKySu\\fontVN\\vps-thanh-hoa.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        //        using (var document = new iTextSharp.text.Document())
        //        {
        //            // Tạo tệp tin PDF và thêm dữ liệu vào
        //            var memoryStream = new MemoryStream();
        //            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        //            document.Open();

        //            PdfPTable table = new PdfPTable(4); // 4 cột trong bảng
        //                                                // ... Thêm dữ liệu vào bảng

        //            document.Add(table);
        //            document.Close();

        //            byte[] bytes = memoryStream.ToArray();

        //            // Gửi email với tệp tin PDF đính kèm
        //            MailMessage message = new MailMessage();
        //            message.From = new MailAddress("sender@example.com");
        //            message.Subject = "Danh sách khách hàng";
        //            message.Body = "Xin chào, dưới đây là danh sách khách hàng.";
        //            message.Attachments.Add(new Attachment(new MemoryStream(bytes), "DanhSachKhachHang.pdf"));

        //            message.To.Add(new MailAddress("recipient@example.com"));

        //            SmtpClient client = new SmtpClient("smtp.example.com", 587, "your_username", "your_password");
        //            client.SecurityOptions = SecurityOptions.Auto;
        //            client.Send(message);

        //            // Trả về tệp tin PDF
        //            return File(bytes, "application/pdf", "DanhSachKhachHang.pdf");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}
        [HttpPut]
        [Route("/api/[controller]/update-khach-hang")]
        public async Task<ActionResult> UpdateKhachHang( KhachHangModel entity)
        {
            try
            {

                //byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                //int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<KhachHangEntity>(entity);
                //mapEntity.CreateBy = userId;
                await _khachHangRepository.UpdateKhachHang( entity);

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
    }
}
