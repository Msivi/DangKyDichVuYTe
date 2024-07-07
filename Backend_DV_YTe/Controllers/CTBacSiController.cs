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
    public class CTBacSiController : ControllerBase
    {
        private readonly ICTBacSiRepository _cTBacSiRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTBacSiController(ICTBacSiRepository CTBacSiRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _cTBacSiRepository = CTBacSiRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-ct-bac-si-by-id")]
        public async Task<ActionResult<CTBacSiEntity>> GetCTBacSiById(int maBS, int maCK)
        {
            try
            {
                var entity = await _cTBacSiRepository.GetCTBacSiById(maBS, maCK);

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
        [Route("/api/[controller]/get-all-ct-bac-si")]
        public async Task<ActionResult<IEnumerable<CTBacSiEntity>>> GetAllCTBacSi()
        {
            try
            {
                var entity = await _cTBacSiRepository.GetAllCTBacSi();

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
        [Route("/api/[controller]/create-ct-bac-si")]
        public async Task<ActionResult<string>> CreateCTBacSi(CTBacSiModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTBacSiEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _cTBacSiRepository.CreateCTBacSi(mapEntity);

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
        [Route("/api/[controller]/update-ct-bac-si")]
        public async Task<ActionResult> UpdateCTBacSi(int maBS, int maCK, CTBacSiModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTBacSiEntity>(entity);
                mapEntity.CreateBy = userId;
                await _cTBacSiRepository.UpdateCTBacSi(maBS, maCK, mapEntity);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Chi tiết nhập updated successfully!"));
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
        [Route("/api/[controller]/delete-ct-bac-si")]
        public async Task<ActionResult<CTBacSiEntity>> DeleteCTBacSi(int maBS, int maCK)
        {

            try
            {

                await _cTBacSiRepository.DeleteCTBacSi(maBS, maCK, false);
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
