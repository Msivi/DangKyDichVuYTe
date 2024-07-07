using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Backend_DV_YTe.Repository;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CTXuatThuocController : ControllerBase
    {
        private readonly ICTXuatThuocRepository _CTXuatThuocRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _cache;
        public CTXuatThuocController(ICTXuatThuocRepository CTXuatThuocRepository, IMapper mapper, IDistributedCache distributedCache, IMemoryCache cache)
        {
            _CTXuatThuocRepository = CTXuatThuocRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _cache = cache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-ct-xuat-thuoc-by-id")]
        public async Task<ActionResult<CTXuatThuocEntity>> GetCTXuatThuocById(int maThuoc, int maXuatThuoc)
        {
            try
            {
                var entity = await _CTXuatThuocRepository.GetCTXuatThuocById(maThuoc, maXuatThuoc);

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
        [Route("/api/[controller]/get-all-ct-xuat-thuoc")]
        public async Task<ActionResult<ICollection<CTXuatThuocEntity>>> GetAllCTXuatThuoc()
        {
            try
            {
                var entity = await _CTXuatThuocRepository.GetAllCTXuatThuoc();

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
        [Route("/api/[controller]/get-ct-xuat-thuoc-thong-ke")]
        public async Task<ActionResult<Dictionary<int, int>>> GenerateCTXuatThuocReport(DateTime startTime, DateTime endTime)
        {
            try
            {
                var report = _CTXuatThuocRepository.GenerateXuatThuocReport(startTime, endTime);
                return Ok(report);
            }
            catch (Exception ex)
            {
                var errorResponse = new BaseResponseModel<int>(
                    statusCode: StatusCodes.Status500InternalServerError,
                    code: "Failed!",
                    message: ex.Message);

                return BadRequest(errorResponse);
            }
        }

        [HttpGet]
        [Route("/api/[controller]/get-ct-xuat-thuoc-file-pdf")]
        public IActionResult DownloadPdfFile()
        {
            try
            {
                byte[] pdfBytes = _CTXuatThuocRepository.DownloadPdfFile();

                // Trả về tệp tin PDF
                return File(pdfBytes, "application/pdf", "DanhSachXuatThuoc" + DateTime.Now + ".pdf");
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
        //public async Task<IActionResult> DownloadPdfFile()
        //{
        //    try
        //    {
        //        var report = _CTXuatThuocRepository.DownloadPdfFile();
        //        return Ok(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new BaseResponseModel<int>(
        //            statusCode: StatusCodes.Status500InternalServerError,
        //            code: "Failed!",
        //            message: ex.Message);

        //        return BadRequest(errorResponse);
        //    }
        //}


        [HttpPost]
        [Route("/api/[controller]/create-ct-xuat-thuoc")]
        public async Task<ActionResult<string>> CreateCTXuatThuoc(CTXuatThuocModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTXuatThuocEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _CTXuatThuocRepository.CreateCTXuatThuoc(mapEntity);

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
        [Route("/api/[controller]/update-ct-xuat-thuoc")]
        public async Task<ActionResult> UpdateCTXuatThuoc(int maThuoc, int maXuatThuoc, CTXuatThuocModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTXuatThuocEntity>(entity);
                mapEntity.CreateBy = userId;
                await _CTXuatThuocRepository.UpdateCTXuatThuoc(maThuoc, maXuatThuoc, mapEntity);

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
        [Route("/api/[controller]/delete-ct-xuat-thuoc")]
        public async Task<ActionResult<CTXuatThuocEntity>> DeleteCTXuatThuoc(int maThuoc, int maXuatThuoc)
        {

            try
            {

                await _CTXuatThuocRepository.DeleteCTXuatThuoc(maThuoc, maXuatThuoc, false);
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
        //=============================================

    }
}
