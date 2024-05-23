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
    [Route("api/[controller]")]
    [ApiController]
    
    public class KetQuaDichVuController : ControllerBase
    {
        private readonly IKetQuaDichVuRepository _ketQuaDichVuRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public KetQuaDichVuController(IKetQuaDichVuRepository KetQuaDichVuRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _ketQuaDichVuRepository = KetQuaDichVuRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-ket-qua-dich-vu-by-id")]
        public async Task<ActionResult<KetQuaDichVuEntity>> GetKetQuaDichVuById(int id)
        {
            try
            {
                var entity = await _ketQuaDichVuRepository.GetKetQuaDichVuById(id);

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
        [Route("/api/[controller]/get-all-ket-qua-dich-vu")]
        public async Task<ActionResult<ICollection<KetQuaDichVuEntity>>> GetAllKetQuaDichVu()
        {
            try
            {
                var entity = await _ketQuaDichVuRepository.GetAllKetQuaDichVu();

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
       // [Authorize(Roles = "KhachHang")]
        [HttpGet]
        [Route("/api/[controller]/get-all-ket-qua-dich-vu-khach-hang")]
        public async Task<ActionResult<ICollection<KetQuaDichVuEntity>>> GetKetQuaDichVuKhachHang()
        {
            try
            {
                var entity = await _ketQuaDichVuRepository.GetKetQuaDichVuByKhachHang();

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
        //[Route("/api/[controller]/search-ket-qua-dich-vu")]
        //public async Task<ActionResult<ICollection<KetQuaDichVuEntity>>> SearchKetQuaDichVu(string searchKey)
        //{
        //    try
        //    {
        //        var KetQuaDichVuList = await _ketQuaDichVuRepository.SearchKetQuaDichVu(searchKey);
        //        if (!KetQuaDichVuList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(KetQuaDichVuList);
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
        [Authorize(Roles = "NhanVien")]
        [HttpPost]
        [Route("/api/[controller]/create-ket-qua-dich-vu")]
        public async Task<ActionResult<string>> CreateKetQuaDichVu(KetQuaDichVuModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<KetQuaDichVuEntity>(model);
                mapEntity.MaNhanVien = userId;
                var result = await _ketQuaDichVuRepository.CreateKetQuaDichVu(mapEntity);

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
        [Route("/api/[controller]/update-ket-qua-dich-vu")]
        public async Task<IActionResult> UpdateKetQuaDichVu(int id, KetQuaDichVuModel entity)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<KetQuaDichVuEntity>(entity);
                //mapEntity.CreateBy=userId;

                await _ketQuaDichVuRepository.UpdateKetQuaDichVu(id, entity);

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
        [Route("/api/[controller]/delete-ket-qua-dich-vu")]
        public async Task<ActionResult<KetQuaDichVuEntity>> DeleteKetQuaDichVu(int keyId)
        {

            try
            {

                await _ketQuaDichVuRepository.DeleteKetQuaDichVu(keyId, false);
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
