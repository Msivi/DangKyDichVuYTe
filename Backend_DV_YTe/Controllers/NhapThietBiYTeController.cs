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

    [Authorize(Roles = "QuanLy")]
    public class NhapThietBiYTeController : ControllerBase
    {
        private readonly INhapThietBiYTeRepository _NhapThietBiYTeRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public NhapThietBiYTeController(INhapThietBiYTeRepository NhapThietBiYTeRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _NhapThietBiYTeRepository = NhapThietBiYTeRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-nhap-thiet-bi-y-te-by-id")]
        public async Task<ActionResult<NhapThietBiYTeEntity>> GetNhapThietBiYTeById(int id)
        {
            try
            {
                var entity = await _NhapThietBiYTeRepository.GetNhapThietBiYTeById(id);

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
        [Route("/api/[controller]/get-all-nhap-thiet-bi-y-te")]
        public async Task<ActionResult<ICollection<NhapThietBiYTeEntity>>> GetAllNhapThietBiYTe()
        {
            try
            {
                var entity = await _NhapThietBiYTeRepository.GetAllNhapThietBiYTe();

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
        //[Route("/api/[controller]/search-nhap-thiet-bi-y-te")]
        //public async Task<ActionResult<IEnumerable<NhapThietBiYTeEntity>>> SearchNhapThietBiYTe(string searchKey)
        //{
        //    try
        //    {
        //        var NhapThietBiYTeList = await _NhapThietBiYTeRepository.SearchNhapThietBiYTe(searchKey);
        //        if (!NhapThietBiYTeList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(NhapThietBiYTeList);
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
        [Route("/api/[controller]/create-nhap-thiet-bi-y-te")]
        public async Task<ActionResult<string>> CreateNhapThietBiYTe(NhapThietBiYTeModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<NhapThietBiYTeEntity>(model);
                mapEntity.MaNhanVien = userId;
                var result = await _NhapThietBiYTeRepository.CreateNhapThietBiYTe(mapEntity);

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
        [Route("/api/[controller]/update-nhap-thiet-bi-y-te")]
        public async Task<ActionResult> UpdateNhapThietBiYTe(int id, NhapThietBiYTeModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<NhapThietBiYTeEntity>(entity);
                mapEntity.MaNhanVien = userId;
                await _NhapThietBiYTeRepository.UpdateNhapThietBiYTe(id, mapEntity);

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
        [Route("/api/[controller]/delete-nhap-thiet-bi-y-te")]
        public async Task<ActionResult<NhapThietBiYTeEntity>> DeleteNhapThietBiYTe(int keyId)
        {

            try
            {

                await _NhapThietBiYTeRepository.DeleteNhapThietBiYTe(keyId, false);
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
