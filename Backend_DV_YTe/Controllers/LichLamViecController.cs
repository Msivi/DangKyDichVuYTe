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
    public class LichLamViecController : ControllerBase
    {
        private readonly ILichLamViecRepository _lichLamViecRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public LichLamViecController(ILichLamViecRepository LichLamViecRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _lichLamViecRepository = LichLamViecRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-lich-lam-viec-by-id")]
        public async Task<ActionResult<LichLamViecEntity>> GetLichLamViecById(int id)
        {
            try
            {
                var entity = await _lichLamViecRepository.GetLichLamViecById(id);

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
        [Route("/api/[controller]/get-lich-lam-viec-by-id-bac-si")]
        public async Task<ActionResult<LichLamViecEntity>> GetLichLamViecByIdBacSi(int maBacSi)
        {
            try
            {
                var entity = await _lichLamViecRepository.GetLichLamViecByIdBacSi(maBacSi);

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
        [Route("/api/[controller]/get-all-lich-lam-viec-bac-si")]
        public async Task<ActionResult<ICollection<LichLamViecEntity>>> GetTTLichLamViecBacSi()
        {
            try
            {
                var entity = await _lichLamViecRepository.GetTTLichLamViecBacSi();

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
        [Route("/api/[controller]/get-all-lich-lam-viec")]
        public async Task<ActionResult<ICollection<LichLamViecEntity>>> GetAllLichLamViec()
        {
            try
            {
                var entity = await _lichLamViecRepository.GetAllLichLamViec();

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
        [Route("/api/[controller]/create-lich-lam-viec")]
        public async Task<ActionResult<string>> CreateLichLamViec(LichLamViecModel model)
        {
            try
            {
                var mapEntity = _mapper.Map<LichLamViecEntity>(model);
             
                var result = await _lichLamViecRepository.CreateLichLamViec(mapEntity);

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
        [Route("/api/[controller]/update-lich-lam-viec")]
        public async Task<ActionResult> UpdateLichLamViec(int id, LichLamViecModel entity)
        {
            try
            {
 
                await _lichLamViecRepository.UpdateLichLamViec(id, entity);

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
        [Route("/api/[controller]/delete-lich-lam-viec")]
        public async Task<ActionResult<LichLamViecEntity>> DeleteLichLamViec(int keyId)
        {

            try
            {

                await _lichLamViecRepository.DeleteLichLamViec(keyId, false);
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
