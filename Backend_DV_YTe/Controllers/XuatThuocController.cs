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
    public class XuatThuocController : ControllerBase
    {
        private readonly IXuatThuocRepository _xuatThuocRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public XuatThuocController(IXuatThuocRepository XuatThuocRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _xuatThuocRepository = XuatThuocRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-xuat-thuoc-by-id")]
        public async Task<ActionResult<XuatThuocEntity>> GetXuatThuocById(int id)
        {
            try
            {
                var entity = await _xuatThuocRepository.GetXuatThuocById(id);

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
        [Route("/api/[controller]/get-all-xuat-thuoc")]
        public async Task<ActionResult<ICollection<XuatThuocEntity>>> GetAllXuatThuoc()
        {
            try
            {
                var entity = await _xuatThuocRepository.GetAllXuatThuoc();

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
        //[Route("/api/[controller]/search-xuat-thuoc")]
        //public async Task<ActionResult<IEnumerable<XuatThuocEntity>>> SearchXuatThuoc(string searchKey)
        //{
        //    try
        //    {
        //        var XuatThuocList = await _xuatThuocRepository.SearchXuatThuoc(searchKey);
        //        if (!XuatThuocList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(XuatThuocList);
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
        [Route("/api/[controller]/create-xuat-thuoc")]
        public async Task<ActionResult<string>> CreateXuatThuoc(XuatThuocModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<XuatThuocEntity>(model);
                mapEntity.MaNhanVien = userId;
                var result = await _xuatThuocRepository.CreateXuatThuoc(mapEntity);

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
        [Route("/api/[controller]/update-xuat-thuoc")]
        public async Task<ActionResult> UpdateXuatThuoc(int id, XuatThuocModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<XuatThuocEntity>(entity);
                mapEntity.MaNhanVien = userId;
                await _xuatThuocRepository.UpdateXuatThuoc(id, mapEntity);

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
        [Route("/api/[controller]/delete-xuat-thuoc")]
        public async Task<ActionResult<XuatThuocEntity>> DeleteXuatThuoc(int keyId)
        {

            try
            {

                await _xuatThuocRepository.DeleteXuatThuoc(keyId, false);
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
