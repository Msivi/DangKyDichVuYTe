using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "QuanLy")]
    public class LoaiThietBiController : ControllerBase
    {
        private readonly ILoaiThietBiRepository _loaiThietBiRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public LoaiThietBiController(ILoaiThietBiRepository LoaiThietBiRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _loaiThietBiRepository = LoaiThietBiRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-loai-thiet-bi-by-id")]
        public async Task<ActionResult<LoaiThietBiEntity>> GetLoaiThietBiById(int id)
        {
            try
            {
                var entity = await _loaiThietBiRepository.GetLoaiThietBiById(id);

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
        [Route("/api/[controller]/get-all-loai-thiet-bi")]
        public async Task<ActionResult<ICollection<LoaiThietBiEntity>>> GetAllLoaiThietBi()
        {
            try
            {
                var entity = await _loaiThietBiRepository.GetAllLoaiThietBi();

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
        //[Route("/api/[controller]/search-loai-thiet-bi")]
        //public async Task<ActionResult<IEnumerable<LoaiThietBiEntity>>> SearchLoaiThietBi(string searchKey)
        //{
        //    try
        //    {
        //        var LoaiThietBiList = await _loaiThietBiRepository.SearchLoaiThietBi(searchKey);
        //        if (!LoaiThietBiList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(LoaiThietBiList);
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

        [HttpPost]
        [Route("/api/[controller]/create-loai-thiet-bi")]
        public async Task<ActionResult<string>> CreateLoaiThietBi(LoaiThietBiModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<LoaiThietBiEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _loaiThietBiRepository.CreateLoaiThietBi(mapEntity);

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
        [Route("/api/[controller]/update-loai-thiet-bi")]
        public async Task<ActionResult> UpdateLoaiThietBi(int id, LoaiThietBiModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<LoaiThietBiEntity>(entity);
                mapEntity.CreateBy = userId;
                await _loaiThietBiRepository.UpdateLoaiThietBi(id, mapEntity);

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
        [Route("/api/[controller]/delete-loai-thiet-bi")]
        public async Task<ActionResult<LoaiThietBiEntity>> DeleteLoaiThietBi(int keyId)
        {

            try
            {

                await _loaiThietBiRepository.DeleteLoaiThietBi(keyId, false);
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
