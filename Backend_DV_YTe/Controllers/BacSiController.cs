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
    public class BacSiController : ControllerBase
    {
        private readonly IBacSiRepository _bacSiRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public BacSiController(IBacSiRepository BacSiRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _bacSiRepository = BacSiRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
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
        public async Task<IActionResult> UpdateBacSi(int id, [FromForm] BacSiModel entity, IFormFile imageFile)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<BacSiEntity>(entity);
                //mapEntity.CreateBy=userId;

                await _bacSiRepository.UpdateBacSi(id, entity,imageFile);

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
    }
}
