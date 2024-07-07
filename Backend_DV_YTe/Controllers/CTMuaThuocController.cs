using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CTMuaThuocController : ControllerBase
    {
        private readonly ICTMuaThuocRepository _CTMuaThuocRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTMuaThuocController(ICTMuaThuocRepository CTMuaThuocRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _CTMuaThuocRepository = CTMuaThuocRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-ct-mua-thuoc-by-id")]
        public async Task<ActionResult<CTMuaThuocEntity>> GetCTMuaThuocById(int maThuoc, int maNhapThuoc)
        {
            try
            {
                var entity = await _CTMuaThuocRepository.GetCTMuaThuocById(maThuoc, maNhapThuoc);

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
        [Route("/api/[controller]/get-all-ct-mua-thuoc")]
        public async Task<ActionResult<ICollection<CTMuaThuocEntity>>> GetAllCTMuaThuoc()
        {
            try
            {
                var entity = await _CTMuaThuocRepository.GetAllCTMuaThuoc();

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
        [Route("/api/[controller]/get-all-ct-mua-thuoc-khach-hang")]
        public async Task<ActionResult<ICollection<CTMuaThuocEntity>>> GetAllCTMuaThuocKH()
        {
            try
            {
                var entity = await _CTMuaThuocRepository.GetAllCTMuaThuocKH();

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
        [Route("/api/[controller]/create-ct-mua-thuoc")]
        public async Task<ActionResult<string>> CreateCTMuaThuoc(CTMuaThuocModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTMuaThuocEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _CTMuaThuocRepository.CreateCTMuaThuoc(mapEntity);

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
        [Route("/api/[controller]/update-ct-mua-thuoc")]
        public async Task<ActionResult> UpdateCTMuaThuoc(int maHD, int maThuoc, CTMuaThuocModel entity)
        {
            try
            {
                 
                await _CTMuaThuocRepository.UpdateCTMuaThuoc(maHD, maThuoc, entity);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Chi tiết nhập vaccine updated successfully!"));
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
        [Route("/api/[controller]/delete-ct-mua-thuoc")]
        public async Task<ActionResult<CTMuaThuocEntity>> DeleteCTMuaThuoc(int maThuoc, int maNhapThuoc)
        {

            try
            {

                await _CTMuaThuocRepository.DeleteCTMuaThuoc(maThuoc, maNhapThuoc, true);
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
