using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BacSiController : ControllerBase
    {
        private readonly IBacSiRepository _bacSiRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BacSiController(IBacSiRepository BacSiRepository, IMapper mapper, IDistributedCache distributedCache,IWebHostEnvironment webHostEnvironment)
        {
            _bacSiRepository = BacSiRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("/api/[controller]/get-bac-si-by-id")]
        public async Task<ActionResult<BacSiEntity>> GetBacSiById(int id)
        {
            try
            {
                var entity = await _bacSiRepository.GetBacSiById(id);

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
        [Route("/api/[controller]/get-all-tt-bac-si-by-id")]
        public async Task<ActionResult<BacSiEntity>> GetAllTTBacSiById()
        {
            try
            {
                var entity = await _bacSiRepository.GetAllTTBacSiById();

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
        //[Route("/api/[controller]/get-all-bac-si")]
        //public async Task<ActionResult<ICollection<BacSiEntity>>> GetAllBacSi()
        //{
        //    try
        //    {
        //        var entity = await _bacSiRepository.GetAllBacSi();

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
        [HttpGet]
        [Route("/api/[controller]/get-all-bac-si")]
        public async Task<ActionResult<ICollection<BacSiEntity>>> GetAllTTBacSi()
        {
            try
            {
                var entity = await _bacSiRepository.GetAllTTBacSi();

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
        [Route("/api/[controller]/get-all-bac-si-by-chuyen-khoa")]
        public async Task<ActionResult<ICollection<BacSiInfoModel>>> GetAllBacSiByChuyenKhoa(string chuyenKhoa)
        {
            try
            {
                var entity = await _bacSiRepository.GetBacSiByChuyenKhoa(chuyenKhoa);

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
        [Route("/api/[controller]/search-bac-si")]
        public async Task<ActionResult<ICollection<BacSiEntity>>> SearchBacSi(string searchKey)
        {
            try
            {
                var BacSiList = await _bacSiRepository.SearchBacSi(searchKey);
                if (!BacSiList.Any())
                {
                    return BadRequest("Not found!");
                }
                return Ok(BacSiList);
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

        [HttpPost]
        [Route("/api/[controller]/create-bac-si")]
        public async Task<ActionResult<string>> CreateBacSi([FromForm] BacSiModel model, IFormFile imageFile)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<BacSiEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _bacSiRepository.CreateBacSi(mapEntity, imageFile);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status201Created,
                    code: "Success!",
                    data: result));
            }
            catch (Exception ex)
            {
                dynamic result;
                result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpPut]
        [Route("/api/[controller]/update-bac-si")]
        public async Task<IActionResult> UpdateBacSi(int id, [FromForm] UpdateBacSiModel entity)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<BacSiEntity>(entity);
                //mapEntity.CreateBy=userId;

                await _bacSiRepository.UpdateBacSi(id, entity);

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
        [Route("/api/[controller]/delete-bac-si")]
        public async Task<ActionResult<BacSiEntity>> DeleteBacSi(int keyId)
        {

            try
            {

                await _bacSiRepository.DeleteBacSi(keyId, false);
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

        [HttpPut]
        [Route("/api/[controller]/update-tkmk-bac-si")]
        public async Task<IActionResult> UpdateBacSiTKMK(int id,BacSiModel entity)
        {
            try
            {
                 

                await _bacSiRepository.UpdateTkMk(id,entity);

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
        [HttpPost("/api/[controller]/upload-avatar-bac-si")]
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


                var user = await _bacSiRepository.GetBacSiById(userId);
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
                if (!string.IsNullOrEmpty(user.hinhAnh))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.ContentRootPath, user.hinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                user.hinhAnh = $"/Images/" + uniqueFileName;
                await _bacSiRepository.UpdateAvatar(user.hinhAnh);

                return Ok(new { message = "Đường dẫn ảnh đại diện đã được cập nhật." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi cập nhật đường dẫn ảnh đại diện: " + ex.Message });
            }
        }
    }
}
