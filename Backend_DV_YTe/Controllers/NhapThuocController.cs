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
    public class NhapThuocController : ControllerBase
    {
        private readonly INhapThuocRepository _nhapThuocRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public NhapThuocController(INhapThuocRepository NhapThuocRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _nhapThuocRepository = NhapThuocRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-nhap-thuoc-by-id")]
        public async Task<ActionResult<NhapThuocEntity>> GetNhapThuocById(int id)
        {
            try
            {
                var entity = await _nhapThuocRepository.GetNhapThuocById(id);

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
        [Route("/api/[controller]/get-all-nhap-thuoc")]
        public async Task<ActionResult<ICollection<NhapThuocEntity>>> GetAllNhapThuoc()
        {
            try
            {
                var entity = await _nhapThuocRepository.GetAllNhapThuoc();

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
        //[Route("/api/[controller]/search-nhap-thuoc")]
        //public async Task<ActionResult<IEnumerable<NhapThuocEntity>>> SearchNhapThuoc(string searchKey)
        //{
        //    try
        //    {
        //        var NhapThuocList = await _nhapThuocRepository.SearchNhapThuoc(searchKey);
        //        if (!NhapThuocList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(NhapThuocList);
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
        [Route("/api/[controller]/create-nhap-thuoc")]
        public async Task<ActionResult<string>> CreateNhapThuoc(NhapThuocModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<NhapThuocEntity>(model);
                mapEntity.MaNhanVien = userId;
                var result = await _nhapThuocRepository.CreateNhapThuoc(mapEntity);

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
        [Route("/api/[controller]/update-nhap-thuoc")]
        public async Task<ActionResult> UpdateNhapThuoc(int id, NhapThuocModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<NhapThuocEntity>(entity);
                mapEntity.MaNhanVien = userId;
                await _nhapThuocRepository.UpdateNhapThuoc(id, mapEntity);

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
        [Route("/api/[controller]/delete-nhap-thuoc")]
        public async Task<ActionResult<NhapThuocEntity>> DeleteNhapThuoc(int keyId)
        {

            try
            {

                await _nhapThuocRepository.DeleteNhapThuoc(keyId, false);
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
