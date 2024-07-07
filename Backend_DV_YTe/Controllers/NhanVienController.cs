using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private readonly INhanVienRepository _nhanVienRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _distributedCache;
        public NhanVienController(INhanVienRepository NhanVienRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment, AppDbContext context, IDistributedCache distributedCache)
        {
            _nhanVienRepository = NhanVienRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _distributedCache = distributedCache;
        }
        [HttpGet]
        [Route("/api/[controller]/get-nhan-vien-by-id")]
        public async Task<ActionResult<NhanVienEntity>> GetNhanVienById(int id)
        {
            try
            {
                var entity = await _nhanVienRepository.GetNhanVienById(id);

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
        //[Authorize(Roles = "QuanLy")]
        [HttpGet]
        [Route("/api/[controller]/get-all-nhan-vien")]
        public async Task<ActionResult<IEnumerable<NhanVienEntity>>> GetAllNhanVien()
        {
            try
            {
                var entity = await _nhanVienRepository.GetAllNhanVien();

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
        //[Authorize(Roles = "QuanLy")]
        [HttpGet]
        [Route("/api/[controller]/search-nhan-vien")]
        public async Task<ActionResult<IEnumerable<NhanVienEntity>>> SearchNhanVien(string searchKey)
        {
            try
            {
                var NhanVienList = await _nhanVienRepository.SearchNhanVien(searchKey);
                if (!NhanVienList.Any())
                {
                    return BadRequest("Not found!");
                }
                return Ok(NhanVienList);
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
        [Route("/api/[controller]/get-tt-nhan-vien")]
        public async Task<ActionResult<ICollection<NhanVienEntity>>> GetTTNhanVien()
        {
            try
            {
                var entity = await _nhanVienRepository.GetTTNhanVien();

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

        [HttpPost("UploadAvatar")]
        public async Task<IActionResult> UploadDescuento(IFormFile avatarFile)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);


                var user = await _nhanVienRepository.GetNhanVienById(userId);
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
                await _nhanVienRepository.UpdateAvatar(user.Avatar);

                return Ok(new { message = "Đường dẫn ảnh đại diện đã được cập nhật." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi cập nhật đường dẫn ảnh đại diện: " + ex.Message });
            }
        }
    }
}
