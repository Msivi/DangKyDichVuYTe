using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichHenController : ControllerBase
    {
        private readonly ILichHenRepository _lichHenRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly AppDbContext _context;
        public LichHenController(ILichHenRepository LichHenRepository, IMapper mapper, IDistributedCache distributedCache, AppDbContext context)
        {
            _lichHenRepository = LichHenRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _context = context;
        }

        [HttpGet]
        [Route("/api/[controller]/get-lich-hen-by-id")]
        public async Task<ActionResult<LichHenEntity>> GetLichHenById(int id)
        {
            try
            {
                var entity = await _lichHenRepository.GetLichHenById(id);

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
        [Route("/api/[controller]/get-all-lich-hen")]
        public async Task<ActionResult<ICollection<LichHenEntity>>> GetAllLichHen()
        {
            try
            {
                var entity = await _lichHenRepository.GetAllLichHen();

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
        [Route("/api/[controller]/get-all-lich-hen-khach-hang")]
        public async Task<ActionResult<ICollection<LichHenEntity>>> GetAllLichHenKhachHang()
        {
            try
            {
                var entity = await _lichHenRepository.GetAllLichHenKhachHang();

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
        //[Route("/api/[controller]/get-all-lich-hen-onine")]
        //public async Task<ActionResult<ICollection<LichHenEntity>>> GetAllLichHenOnline()
        //{
        //    try
        //    {
        //        var entity = await _lichHenRepository.GetAllLichHenOnline();

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

        //[HttpGet]
        //[Route("/api/[controller]/search-lich-hen")]
        //public async Task<ActionResult<ICollection<LichHenEntity>>> SearchLichHen(string searchKey)
        //{
        //    try
        //    {
        //        var LichHenList = await _lichHenRepository.SearchLichHen(searchKey);
        //        if (!LichHenList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(LichHenList);
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
        [Route("/api/[controller]/create-lich-hen")]
        public async Task<ActionResult<string>> CreateLichHen(LichHenModel entity)
        {
            try
            {

                
                var mapEntity = _mapper.Map<LichHenEntity>(entity);
               

                var result = await _lichHenRepository.CreateLichHen(mapEntity);

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
        [Route("/api/[controller]/update-lich-hen")]
        public async Task<IActionResult> UpdateLichHen(int id, LichHenModel entity)
        {
            try
            {
                //byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
                //int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<LichHenEntity>(entity);
                //mapEntity.CreateBy = userId;

                await _lichHenRepository.UpdateLichHen(id, entity);

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
        [HttpPut]
        [Route("/api/[controller]/update-lich-hen-nhan-vien")]
        public async Task<IActionResult> UpdateLichHenNhanVien(int id, string diaDiem, DateTime thoiGianDuKien)
        {
            try
            {
                //byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
                //int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<LichHenEntity>(entity);
                //mapEntity.CreateBy = userId;

                await _lichHenRepository.UpdateLichHenNhanVien(id, diaDiem, thoiGianDuKien);

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
        [Route("/api/[controller]/delete-lich-hen")]
        public async Task<ActionResult<LichHenEntity>> DeleteLichHen(int keyId)
        {

            try
            {

                await _lichHenRepository.DeleteLichHen(keyId, false);
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
