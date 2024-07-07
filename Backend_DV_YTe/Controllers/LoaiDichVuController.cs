using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;

namespace Backend_DV_YTe.Controllers
{
    //[Authorize(Roles = "QuanLy")]
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiDichVuController : ControllerBase
    {
        private readonly ILoaiDichVuRepository _LoaiDichVuRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public LoaiDichVuController(ILoaiDichVuRepository LoaiDichVuRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _LoaiDichVuRepository = LoaiDichVuRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-loai-dich-vu-by-id")]
        public async Task<ActionResult<LoaiDichVuEntity>> GetLoaiDichVuById(int id)
        {
            try
            {
                var entity = await _LoaiDichVuRepository.GetLoaiDichVuById(id);

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
        [Route("/api/[controller]/get-all-loai-dich-vu")]
        public async Task<ActionResult<ICollection<LoaiDichVuEntity>>> GetAllLoaiDichVu()
        {
            try
            {
                var entity = await _LoaiDichVuRepository.GetAllLoaiDichVu();

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
        //[Route("/api/[controller]/search-loai-dich-vu")]
        //public async Task<ActionResult<IEnumerable<LoaiDichVuEntity>>> SearchLoaiDichVu(string searchKey)
        //{
        //    try
        //    {
        //        var LoaiDichVuList = await _LoaiDichVuRepository.SearchLoaiDichVu(searchKey);
        //        if (!LoaiDichVuList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(LoaiDichVuList);
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
        [Route("/api/[controller]/create-loai-dich-vu")]
        public async Task<ActionResult<string>> CreateLoaiDichVu(LoaiDichVuModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

               
                var mapEntity = _mapper.Map<LoaiDichVuEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _LoaiDichVuRepository.CreateLoaiDichVu(mapEntity);

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
        [Route("/api/[controller]/update-loai-dich-vu")]
        public async Task<ActionResult> UpdateLoaiDichVu(int id, LoaiDichVuModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<LoaiDichVuEntity>(entity);
                mapEntity.CreateBy = userId;
                await _LoaiDichVuRepository.UpdateLoaiDichVu(id, mapEntity);

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
        [Route("/api/[controller]/delete-loai-dich-vu")]
        public async Task<ActionResult<LoaiDichVuEntity>> DeleteLoaiDichVu(int keyId)
        {

            try
            {

                await _LoaiDichVuRepository.DeleteLoaiDichVu(keyId, false);
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
