using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IKhachHangRepository _KhachHangRepository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDistributedCache _distributedCache;
        public AuthenticationController(IMapper mapper, IAuthenticationService authenticationService,IKhachHangRepository khachHangRepository, IDistributedCache distributedCache)
        {
         
            _mapper = mapper;
            _authenticationService = authenticationService;
            _KhachHangRepository= khachHangRepository;
            _distributedCache = distributedCache;
        }
        [HttpPost]
        [Route("/api/[controller]/login")]
        public async Task<IActionResult> Login(LoginModel model, [FromServices] IDistributedCache distributedCache)
        {
            try
            {
                var result = await _authenticationService.Login(model);

                if (result is KhachHangEntity khachhang)
                {
                    var token = _authenticationService.GenerateToken(khachhang);

                    var userIdBytes = BitConverter.GetBytes(khachhang.maKhachHang);
                    await distributedCache.SetAsync("UserId", userIdBytes);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Authentication successful",
                        Data = token
                    });
                }
                else if (result is NhanVienEntity nhanvien)
                {
                    var token = _authenticationService.GenerateToken(nhanvien);

                    var userIdBytes = BitConverter.GetBytes(nhanvien.Id);
                    await distributedCache.SetAsync("UserId", userIdBytes);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Authentication successful",
                        Data = token
                    });
                }
                else if (result is BacSiEntity bacSi)
                {
                    var token = _authenticationService.GenerateToken(bacSi);

                    var userIdBytes = BitConverter.GetBytes(bacSi.Id);
                    await distributedCache.SetAsync("UserId", userIdBytes);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Authentication successful",
                        Data = token
                    });
                }

                return BadRequest(new ApiResponse { Success = false, Message = "Unauthorized access" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("/api/[controller]/create-khach-hang")]
        public async Task<ActionResult<KhachHangEntity>> CreateKhachHang(KhachHangModel model)
        {
            try
            {


                var result = await _authenticationService.CreateKhachHang(model);

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
        [HttpPost]
        [Route("/api/[controller]/create-bac-si")]
        public async Task<ActionResult<BacSiEntity>> CreateBacSi(BacSiModel model)
        {
            try
            {


                var result = await _authenticationService.CreateBacSi(model);

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

        //[Authorize(Roles = "QuanLy")]
        [HttpPost]
        [Route("/api/[controller]/create-nhan-vien")]
        public async Task<ActionResult<KhachHangEntity>> CreateNhanVien(NhanVienModel model)
        {
            try
            {
                var result = await _authenticationService.CreateNhanVien(model);

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
        //[Authorize(Roles = "QuanLy")]
        [HttpPost]
        [Route("/api/[controller]/create-quan-ly")]
        public async Task<ActionResult<KhachHangEntity>> CreateQuanLy(NhanVienModel model)
        {
            try
            {
                var result = await _authenticationService.CreateQuanLy(model);

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

        [HttpPost]
        [Route("/api/[controller]/change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                await _authenticationService.ChangePassword(changePasswordRequest);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status201Created,
                    code: "Success!",
                    data: "Password changed successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Error",
                    message: ex.Message));
            }
        }

        private List<string> _tokenBlacklist = new List<string>();

        [HttpPost]
        [Route("/api/[controller]/logout")]
        public async Task<IActionResult> LogoutAsync([FromServices] IDistributedCache distributedCache)
        {
            byte[] userIdBytes = await distributedCache.GetAsync("UserId");
            if (userIdBytes != null)
            {
                string userId = Encoding.UTF8.GetString(userIdBytes);

                // Xóa giá trị UserId từ Distributed Cache
                await distributedCache.RemoveAsync("UserId");
                // Lấy mã token hiện tại từ header Authorization
                var currentToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Thêm mã token vào danh sách blacklist
                _tokenBlacklist.Add(currentToken);
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Logout successful"
            });
        }
    }
}
