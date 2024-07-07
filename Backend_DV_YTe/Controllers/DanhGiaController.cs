using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhGiaController : ControllerBase
    {
        private readonly IDanhGiaRepository _danhGiaRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public DanhGiaController(IDanhGiaRepository DanhGiaRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _danhGiaRepository = DanhGiaRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-danh-gia-by-id")]
        public async Task<ActionResult<DanhGiaEntity>> GetDanhGiaById(int id)
        {
            try
            {
                var entity = await _danhGiaRepository.GetDanhGiaById(id);

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
        [Route("/api/[controller]/get-all-danh-gia-chua-duyet")]
        public async Task<ActionResult<ICollection<DanhGiaEntity>>> GetAllDanhGiaChuaDuyet()
        {
            try
            {
                var entity = await _danhGiaRepository.GetAllDanhGiaChuaDuyet();

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
        [Route("/api/[controller]/get-all-danh-gia-da-duyet")]
        public async Task<ActionResult<ICollection<DanhGiaEntity>>> GetAllDanhGiaDaDuyet()
        {
            try
            {
                var entity = await _danhGiaRepository.GetAllDanhGiaDaDuyet();

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
        [Route("/api/[controller]/get-sao-danh-gia")]
        public async Task<ActionResult<ICollection<DichVuDanhGia>>> GetSaoDanhGia(int maDichVu)
        {
            try
            {
                var entity = await _danhGiaRepository.GetDichVuDanhGia(maDichVu);

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
        [Route("/api/[controller]/get-all-danh-gia-by-ma-dich-vu")]
        public async Task<ActionResult<ICollection<DichVuDanhGia>>> GetAllDanhGiaByMaDV(int maDichVu)
        {
            try
            {
                var entity = await _danhGiaRepository.GetAllDanhGiaByMaDichVu(maDichVu);

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
        //[Route("/api/[controller]/search-danh-gia")]
        //public async Task<ActionResult<ICollection<DanhGiaEntity>>> SearchDanhGia(string searchKey)
        //{
        //    try
        //    {
        //        var DanhGiaList = await _danhGiaRepository.SearchDanhGia(searchKey);
        //        if (!DanhGiaList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(DanhGiaList);
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
        [Route("/api/[controller]/create-danh-gia")]
        public async Task<IActionResult> CreateDanhGia([FromForm] DanhGiaModel model, IFormFile? imageFile)
        {
            try
            {



                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<DanhGiaEntity>(model);
                mapEntity.CreateBy = userId;


                var result = await _danhGiaRepository.CreateDanhGia(mapEntity, imageFile);

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
        [Route("/api/[controller]/update-danh-gia")]
        public async Task<IActionResult> UpdateDanhGia(int id, [FromForm] DanhGiaModel entity, IFormFile imageFile)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<DanhGiaEntity>(entity);
                //mapEntity.CreateBy=userId;

                await _danhGiaRepository.UpdateDanhGia(id, entity, imageFile);

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
        //[Authorize(Roles = "NhanVien")]
        [HttpPut]
        [Route("/api/[controller]/update-duyet-danh-gia")]
        public async Task<IActionResult> UpdateDuyetDanhGia(List<int> ids)
        {
            try
            {
                await _danhGiaRepository.DuyetDanhGia(ids);

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
        [Route("/api/[controller]/delete-danh-gia")]
        public async Task<ActionResult<DanhGiaEntity>> DeleteDanhGia(int keyId)
        {

            try
            {

                await _danhGiaRepository.DeleteDanhGia(keyId, false);
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
        [Route("/api/[controller]/get-thong-ke-danh-gia-bac-si")]
        public async Task<ActionResult<Dictionary<int, (string DoctorName, double AverageRating)>>> GetAverageDoctorRating()
        {
            try
            {
              var ketQua=  await _danhGiaRepository.GetAverageRatingPerDoctorWithNames();
                var doctorRatings = ketQua.ToDictionary(
                                       kvp => kvp.Key,
                                       kvp => new DoctorRatingDataModel
                                       {
                                           DoctorName = kvp.Value.DoctorName,
                                           AverageRating = kvp.Value.AverageRating
                                       });
                return Ok(doctorRatings);
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
    }
}
