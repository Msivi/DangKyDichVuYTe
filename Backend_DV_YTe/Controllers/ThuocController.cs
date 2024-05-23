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
    [Authorize(Roles = "QuanLy")]
    public class ThuocController : ControllerBase
    {
        private readonly IThuocRepository _thuocRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public ThuocController(IThuocRepository ThuocRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _thuocRepository = ThuocRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-thuoc-by-id")]
        public async Task<ActionResult<ThuocEntity>> GetThuocById(int id)
        {
            try
            {
                var entity = await _thuocRepository.GetThuocById(id);

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
        [Route("/api/[controller]/get-thuoc-by-id-loai-thuoc")]
        public async Task<ActionResult<ThuocEntity>> GetThuocByIdLoaiThuoc(int id)
        {
            try
            {
                var entity = await _thuocRepository.GetThuocByLoaiThuoc(id);

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
        [Route("/api/[controller]/get-ct-xuat-thuoc-by-loai-pdf")]
        public async Task< IActionResult> DownloadPdfFile(int maLoaiThuoc)
        {
            try
            {
                byte[] pdfBytes = await _thuocRepository.DownloadPdfFile(maLoaiThuoc);

                // Trả về tệp tin PDF
                return File(pdfBytes, "application/pdf", "DanhSachThuocTheoLoai" + DateTime.Now + ".pdf");
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
        [HttpGet]
        [Route("/api/[controller]/get-all-thuoc")]
        public async Task<ActionResult<ICollection<ThuocEntity>>> GetAllThuoc()
        {
            try
            {
                var entity = await _thuocRepository.GetAllThuoc();

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
        [Route("/api/[controller]/search-thuoc")]
        public async Task<ActionResult<IEnumerable<ThuocEntity>>> SearchThuoc(string searchKey)
        {
            try
            {
                var ThuocList = await _thuocRepository.SearchThuoc(searchKey);
                if (!ThuocList.Any())
                {
                    return BadRequest("Not found!");
                }
                return Ok(ThuocList);
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
        [Route("/api/[controller]/create-thuoc")]
        public async Task<ActionResult<string>> CreateThuoc(ThuocModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<ThuocEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _thuocRepository.CreateThuoc(mapEntity);

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
        [Route("/api/[controller]/update-thuoc")]
        public async Task<ActionResult> UpdateThuoc(int id, ThuocModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<ThuocEntity>(entity);
                mapEntity.CreateBy = userId;
                await _thuocRepository.UpdateThuoc(id, mapEntity);

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
        [Route("/api/[controller]/delete-thuoc")]
        public async Task<ActionResult<ThuocEntity>> DeleteThuoc(int keyId)
        {

            try
            {

                await _thuocRepository.DeleteThuoc(keyId, false);
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
