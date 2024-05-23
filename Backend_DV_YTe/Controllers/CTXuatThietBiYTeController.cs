using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Backend_DV_YTe.Repository;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CTXuatThietBiYTeController : ControllerBase
    {
        private readonly ICTXuatThietBiYTeRepository _CTXuatThietBiYTeRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTXuatThietBiYTeController(ICTXuatThietBiYTeRepository CTXuatThietBiYTeRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _CTXuatThietBiYTeRepository = CTXuatThietBiYTeRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
            
        }
        //List<CTXuatThietBiYTeEntity> xuatThietBiList = ICTXuatThietBiYTeRepository.danhSachXuatThietBi;

        [HttpGet]
        [Route("/api/[controller]/get-ct-xuat-thiet-bi-y-te-by-id")]
        public async Task<ActionResult<CTXuatThietBiYTeEntity>> GetCTXuatThietBiYTeById(int maThuoc, int maXuatThuoc)
        {
            try
            {
                var entity = await _CTXuatThietBiYTeRepository.GetCTXuatThietBiYTeById(maThuoc, maXuatThuoc);

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
        [Route("/api/[controller]/get-all-ct-xuat-thiet-bi-y-te")]
        public async Task<ActionResult<IEnumerable<CTXuatThietBiYTeEntity>>> GetAllCTXuatThietBiYTe()
        {
            try
            {
                var entity = await _CTXuatThietBiYTeRepository.GetAllCTXuatThietBiYTe();

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
        [Route("/api/[controller]/get-ct-xuat-thiet-bi-y-te-report")]
        public async Task<ActionResult<Dictionary<int, int>>> GenerateCTXuatThietBiYTeReport(DateTime startTime, DateTime endTime)
        {
            try
            {
                var report = _CTXuatThietBiYTeRepository.GenerateXuatThuocReport(startTime, endTime);
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
        //=====================================
 
        [HttpGet]
        [Route("/api/[controller]/get-ct-xuat-thiet-bi-file-pdf")]
        public IActionResult DownloadPdfFile()
        {
            try
            {
                byte[] pdfBytes = _CTXuatThietBiYTeRepository.DownloadPdfFile();

                // Trả về tệp tin PDF
                return File(pdfBytes, "application/pdf", "DanhSachXuatThietBi" + DateTime.Now + ".pdf");
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
        //===========================================


        [HttpPost]
        [Route("/api/[controller]/create-ct-xuat-thiet-bi-y-te")]
        public async Task<ActionResult<string>> CreateCTXuatThietBiYTe(CTXuatThietBiYTeModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTXuatThietBiYTeEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _CTXuatThietBiYTeRepository.CreateCTXuatThietBiYTe(mapEntity);

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
        [Route("/api/[controller]/update-ct-xuat-thiet-bi-y-te")]
        public async Task<ActionResult> UpdateCTXuatThietBiYTe(int maThietBi, int maXuatThietBi, CTXuatThietBiYTeModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTXuatThietBiYTeEntity>(entity);
                mapEntity.CreateBy = userId;
                await _CTXuatThietBiYTeRepository.UpdateCTXuatThietBiYTe(maThietBi, maXuatThietBi, mapEntity);

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
        [Route("/api/[controller]/delete-ct-xuat-thiet-bi-y-te")]
        public async Task<ActionResult<CTXuatThietBiYTeEntity>> DeleteCTXuatThietBiYTe(int maThietBi, int maXuatThietBi)
        {

            try
            {

                await _CTXuatThietBiYTeRepository.DeleteCTXuatThietBiYTe(maThietBi, maXuatThietBi, false);
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
