using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public HoaDonController(IHoaDonRepository HoaDonRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _hoaDonRepository = HoaDonRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        [HttpGet]
        [Route("/api/[controller]/get-all-hoa-don")]
        public async Task<IActionResult> GetAllTTHoaDon()
        {
            try
            {
                var entity = await _hoaDonRepository.GetAllHoaDon();

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
        [HttpPost]
        [Route("/api/[controller]/create-hoa-don")]
        public async Task<ActionResult<string>> CreateHoaDon()
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                
                var result = await _hoaDonRepository.CreateHoaDon();

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
        [HttpDelete]
        [Route("/api/[controller]/delete-hoa-don")]
        public async Task<ActionResult<HoaDonEntity>> DeleteHoaDon(int keyId)
        {

            try
            {

                await _hoaDonRepository.DeleteHoaDon(keyId, false);
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
    }
}
