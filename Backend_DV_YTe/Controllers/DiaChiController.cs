using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaChiController : ControllerBase
    {
        private readonly IDiaChiRepository _diaChiRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public DiaChiController(IDiaChiRepository DiaChiRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _diaChiRepository = DiaChiRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-dia-chi-by-id")]
        public async Task<ActionResult<DiaChiEntity>> GetDiaChiById(int id)
        {
            try
            {
                var entity = await _diaChiRepository.GetDiaChiById(id);

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
        [Route("/api/[controller]/get-all-dia-chi")]
        public async Task<ActionResult<ICollection<DiaChiEntity>>> GetAllDiaChi()
        {
            try
            {
                var entity = await _diaChiRepository.GetAllDiaChi();

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
        //[Route("/api/[controller]/search-dia-chi")]
        //public async Task<ActionResult<IEnumerable<DiaChiEntity>>> SearchDiaChi(string searchKey)
        //{
        //    try
        //    {
        //        var DiaChiList = await _diaChiRepository.SearchDiaChi(searchKey);
        //        if (!DiaChiList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(DiaChiList);
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic result = new BaseResponseModel<string>(
        //            statusCode: StatusCodes.Status500InternalServerError,
        //            code: "Failed!",
        //            message: ex.Message);
        //        return BadRequest(result);
        //    }
        //}
        [HttpGet]
        [Route("/api/[controller]/get-dia-chi-defalt")]
        public async Task<ActionResult<ICollection<DiaChiEntity>>> GetDefaultAddressAsync()
        {
            try
            {
                var entity = await _diaChiRepository.GetDefaultAddressAsync();

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
        [Route("/api/[controller]/create-dia-chi")]
        public async Task<ActionResult<string>> CreateDiaChi(DiaChiModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<DiaChiEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _diaChiRepository.CreateDiaChi(mapEntity);

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

        [HttpPost("/api/[controller]/create-dia-chi-default")]
        public async Task<IActionResult> SetDefaultDiaChi(int id)
        {
            try
            {
               var result = await _diaChiRepository.SetDefaultAddressAsync(id);
                
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
        [Route("/api/[controller]/update-dia-chi")]
        public async Task<ActionResult> UpdateDiaChi(int id, DiaChiModel entity)
        {
            try
            {

                 

                
                await _diaChiRepository.UpdateDiaChi(id, entity);

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
        [Route("/api/[controller]/delete-dia-chi")]
        public async Task<ActionResult<DiaChiEntity>> DeleteDiaChi(int keyId)
        {

            try
            {

                await _diaChiRepository.DeleteDiaChi(keyId, false);
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
