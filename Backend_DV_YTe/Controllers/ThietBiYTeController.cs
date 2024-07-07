using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
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
    
    public class ThietBiYTeController : ControllerBase
    {
        private readonly IThietBiYteRepository _thietBiYTeRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public ThietBiYTeController(IThietBiYteRepository ThietBiYTeRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _thietBiYTeRepository = ThietBiYTeRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-thiet-bi-y-te-by-id")]
        public async Task<ActionResult<AllThietBiModel>> GetThietBiYTeById(int id)
        {
            try
            {
                var entity = await _thietBiYTeRepository.GetThietBiYTeById(id);

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
        [Route("/api/[controller]/get-all-thiet-bi-y-te")]
        public async Task<ActionResult<ICollection<AllThietBiModel>>> GetAllThietBiYTe()
        {
            try
            {
                var entity = await _thietBiYTeRepository.GetAllThietBiYTe();

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
        [Route("/api/[controller]/get-all-thiet-bi-by-ma-loai")]
        public async Task<ActionResult<ICollection<AllThietBiModel>>> GetAllThietBiYTe(int maLoaiThietBi)
        {
            try
            {
                var entity = await _thietBiYTeRepository.GetAllThietBiByMaLoaiTB(maLoaiThietBi);

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
        [Route("/api/[controller]/search-thiet-bi-y-te")]
        public async Task<ActionResult<IEnumerable<ThietBiYTeEntity>>> SearchThietBiYTe(string searchKey)
        {
            try
            {
                var ThietBiYTeList = await _thietBiYTeRepository.SearchThietBiYTe(searchKey);
                if (!ThietBiYTeList.Any())
                {
                    return BadRequest("Not found!");
                }
                return Ok(ThietBiYTeList);
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
        //[Authorize(Roles = "QuanLy")]
        [HttpPost]
        [Route("/api/[controller]/create-thiet-bi-y-te")]
        public async Task<ActionResult<string>> CreateThietBiYTe([FromForm] ThietBiYTeModel model, IFormFile? imageFile)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<ThietBiYTeEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _thietBiYTeRepository.CreateThietBiYTe(mapEntity,imageFile);

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
        //[Authorize(Roles = "QuanLy")]
        [HttpPut]
        [Route("/api/[controller]/update-thiet-bi-y-te")]
        public async Task<ActionResult> UpdateThietBiYTe(int id,[FromForm] ThietBiYTeModel entity, IFormFile? imageFile)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<ThietBiYTeEntity>(entity);
                mapEntity.CreateBy = userId;
                await _thietBiYTeRepository.UpdateThietBiYTe(id, mapEntity,imageFile);

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
        //[Authorize(Roles = "QuanLy")]
        [HttpDelete]
        [Route("/api/[controller]/delete-thiet-bi-y-te")]
        public async Task<ActionResult<ThietBiYTeEntity>> DeleteThietBiYTe(int keyId)
        {

            try
            {

                await _thietBiYTeRepository.DeleteThietBiYTe(keyId, false);
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

        [HttpGet]
        [Route("/api/[controller]/get-ct-xuat-thiet-bi-by-loai-pdf")]
        public async Task<IActionResult> DownloadPdfFile(int maLoaiThietBi)
        {
            try
            {
                byte[] pdfBytes = await _thietBiYTeRepository.DownloadPdfFile(maLoaiThietBi);

                // Trả về tệp tin PDF
                return File(pdfBytes, "application/pdf", "DanhSachThietBiTheoLoai" + DateTime.Now + ".pdf");
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
    }
}
