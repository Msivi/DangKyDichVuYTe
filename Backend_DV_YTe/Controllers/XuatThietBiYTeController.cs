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
    public class XuatThietBiYTeController : ControllerBase
    {
        private readonly IXuatThietBiYTeRepository _XuatThietBiYTeRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public XuatThietBiYTeController(IXuatThietBiYTeRepository XuatThietBiYTeRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _XuatThietBiYTeRepository = XuatThietBiYTeRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-xuat-thiet-bi-y-te-by-id")]
        public async Task<ActionResult<XuatThietBiYTeEntity>> GetXuatThietBiYTeById(int id)
        {
            try
            {
                var entity = await _XuatThietBiYTeRepository.GetXuatThietBiYTeById(id);

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
        [Route("/api/[controller]/get-all-xuat-thiet-bi-y-te")]
        public async Task<ActionResult<ICollection<XuatThietBiYTeEntity>>> GetAllXuatThietBiYTe()
        {
            try
            {
                var entity = await _XuatThietBiYTeRepository.GetAllXuatThietBiYTe();

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
        //[Route("/api/[controller]/search-xuat-thiet-bi-y-te")]
        //public async Task<ActionResult<IEnumerable<XuatThietBiYTeEntity>>> SearchXuatThietBiYTe(string searchKey)
        //{
        //    try
        //    {
        //        var XuatThietBiYTeList = await _XuatThietBiYTeRepository.SearchXuatThietBiYTe(searchKey);
        //        if (!XuatThietBiYTeList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(XuatThietBiYTeList);
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
        [Route("/api/[controller]/create-xuat-thiet-bi-y-te")]
        public async Task<ActionResult<string>> CreateXuatThietBiYTe(XuatThietBiYTeModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<XuatThietBiYTeEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _XuatThietBiYTeRepository.CreateXuatThietBiYTe(mapEntity);

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
        [Route("/api/[controller]/update-xuat-thiet-bi-y-te")]
        public async Task<ActionResult> UpdateXuatThietBiYTe(int id, XuatThietBiYTeModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<XuatThietBiYTeEntity>(entity);
                mapEntity.CreateBy = userId;
                await _XuatThietBiYTeRepository.UpdateXuatThietBiYTe(id, mapEntity);

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
        [Route("/api/[controller]/delete-xuat-thiet-bi-y-te")]
        public async Task<ActionResult<XuatThietBiYTeEntity>> DeleteXuatThietBiYTe(int keyId)
        {

            try
            {

                await _XuatThietBiYTeRepository.DeleteXuatThietBiYTe(keyId, false);
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
