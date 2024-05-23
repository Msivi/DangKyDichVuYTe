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
    public class ChuyenKhoaController : ControllerBase
    {
        private readonly IChuyenKhoaRepository _chuyenKhoaRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public ChuyenKhoaController(IChuyenKhoaRepository ChuyenKhoaRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _chuyenKhoaRepository = ChuyenKhoaRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-chuyen-khoa-by-id")]
        public async Task<ActionResult<ChuyenKhoaEntity>> GetChuyenKhoaById(int id)
        {
            try
            {
                var entity = await _chuyenKhoaRepository.GetChuyenKhoaById(id);

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
        [Route("/api/[controller]/get-all-chuyen-khoa")]
        public async Task<ActionResult<ICollection<ChuyenKhoaEntity>>> GetAllChuyenKhoa()
        {
            try
            {
                var entity = await _chuyenKhoaRepository.GetAllChuyenKhoa();

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
        [Route("/api/[controller]/search-chuyen-khoa")]
        public async Task<ActionResult<IEnumerable<ChuyenKhoaEntity>>> SearchChuyenKhoa(string searchKey)
        {
            try
            {
                var ChuyenKhoaList = await _chuyenKhoaRepository.SearchChuyenKhoa(searchKey);
                if (!ChuyenKhoaList.Any())
                {
                    return BadRequest("Not found!");
                }
                return Ok(ChuyenKhoaList);
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
        [Route("/api/[controller]/create-chuyen-khoa")]
        public async Task<ActionResult<string>> CreateChuyenKhoa(ChuyenKhoaModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<ChuyenKhoaEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _chuyenKhoaRepository.CreateChuyenKhoa(mapEntity);

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
        [Route("/api/[controller]/update-chuyen-khoa")]
        public async Task<ActionResult> UpdateChuyenKhoa(int id, ChuyenKhoaModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<ChuyenKhoaEntity>(entity);
                mapEntity.CreateBy = userId;
                await _chuyenKhoaRepository.UpdateChuyenKhoa(id, mapEntity);

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
        [Route("/api/[controller]/delete-chuyen-khoa")]
        public async Task<ActionResult<ChuyenKhoaEntity>> DeleteChuyenKhoa(int keyId)
        {

            try
            {

                await _chuyenKhoaRepository.DeleteChuyenKhoa(keyId, false);
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
