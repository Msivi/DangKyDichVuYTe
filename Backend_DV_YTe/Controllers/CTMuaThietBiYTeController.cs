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
    public class CTMuaThietBiYTeController : ControllerBase
    {
        private readonly ICTMuaThietBiYTeRepository _cTMuaThietBiYTeRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTMuaThietBiYTeController(ICTMuaThietBiYTeRepository CTMuaThietBiYTeRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _cTMuaThietBiYTeRepository = CTMuaThietBiYTeRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-ct-mua-thiet-bi-y-te-by-id")]
        public async Task<ActionResult<CTMuaThietBiYTeEntity>> GetCTMuaThietBiYTeById(int maThuoc, int maNhapThuoc)
        {
            try
            {
                var entity = await _cTMuaThietBiYTeRepository.GetCTMuaThietBiYTeById(maThuoc, maNhapThuoc);

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
        [Route("/api/[controller]/get-all-ct-mua-thiet-bi-y-te")]
        public async Task<ActionResult<IEnumerable<CTMuaThietBiYTeEntity>>> GetAllCTMuaThietBiYTe()
        {
            try
            {
                var entity = await _cTMuaThietBiYTeRepository.GetAllCTMuaThietBiYTe();

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
        [Route("/api/[controller]/create-ct-mua-thiet-bi-y-te")]
        public async Task<ActionResult<string>> CreateCTMuaThietBiYTe(CTMuaThietBiYTeModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTMuaThietBiYTeEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _cTMuaThietBiYTeRepository.CreateCTMuaThietBiYTe(mapEntity);

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
        [Route("/api/[controller]/update-ct-mua-thiet-bi-y-te")]
        public async Task<ActionResult> UpdateCTMuaThietBiYTe(int maHD, int maThuoc, CTMuaThietBiYTeModel entity)
        {
            try
            {

                await _cTMuaThietBiYTeRepository.UpdateCTMuaThietBiYTe(maHD, maThuoc, entity);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Chi tiết nhập vaccine updated successfully!"));
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
        [Route("/api/[controller]/delete-ct-mua-thiet-bi-y-te")]
        public async Task<ActionResult<CTMuaThietBiYTeEntity>> DeleteCTMuaThietBiYTe(int maHD, int maThietBi)
        {

            try
            {

                await _cTMuaThietBiYTeRepository.DeleteCTMuaThietBiYTe(maHD, maThietBi, true);
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
