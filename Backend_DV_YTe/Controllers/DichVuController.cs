using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DichVuController : ControllerBase
    {
        private readonly IDichVuRepository _dichVuRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public DichVuController(IDichVuRepository DichVuRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _dichVuRepository = DichVuRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-dich-vu-by-id")]
        public async Task<ActionResult<DichVuEntity>> GetDichVuById(int id)
        {
            try
            {
                var entity = await _dichVuRepository.GetDichVuById(id);

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
        [Route("/api/[controller]/get-all-dich-vu")]
        public async Task<ActionResult<ICollection<DichVuEntity>>> GetAllDichVu()
        {
            try
            {
                var entity = await _dichVuRepository.GetAllDichVu();

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
        [Route("/api/[controller]/get-all-thong-ke-dich-vu")]
        public async Task<IActionResult> GetAllThongKeDichVu(DateTime ngayBD, DateTime ngayKT)
        {
            try
            {
                double tongGiaTri = await _dichVuRepository.thongKeTinhTongGiaTriDichVu(ngayBD, ngayKT);
                //double giaTriTrungBinh = await _dichVuRepository.TinhGiaTriTrungBinhDichVuTrongKhoangThoiGian(ngayBD, ngayKT);
               var DichVuSuDung = await _dichVuRepository.TimDichVuDaSuDung(ngayBD, ngayKT);
                //int dichVuMin = await _dichVuRepository.TimDichVuGiaTriThapNhat(ngayBD, ngayKT);

                var result = new
                {
                    TongGiaTri = tongGiaTri,
                    //GiaTriTrungBinh = giaTriTrungBinh,
                    CountDichVu = DichVuSuDung,
                    //DichVuMin = dichVuMin
                };

                return Ok(result);
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
        //[Route("/api/[controller]/get-all-dich-vu-theo-chuyen-khoa")]
        //public async Task<ActionResult<ICollection<DichVuEntity>>> GetAllDichVuTheoChuyenKhoa(string chuyenKhoa)
        //{
        //    try
        //    {
        //        var entity = await _dichVuRepository.GetAllDichVuTheoChuyenKhoa(chuyenKhoa);

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
        [HttpGet]
        [Route("/api/[controller]/get-all-dich-vu-theo-loai-theo-chuyen-khoa")]
        public async Task<ActionResult<ICollection<DichVuTheoLoaiModel>>> GetAllDichVuTheoLoai(int? loaiDichVuId, int chuyenKhoaId)
        {
            try
            {
                var entity = await _dichVuRepository.GetDichVuByLoaiDichVuAsync(loaiDichVuId, chuyenKhoaId);

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
        [Route("/api/[controller]/search-dich-vu")]
        public async Task<ActionResult<IEnumerable<DichVuEntity>>> SearchDichVu(string searchKey)
        {
            try
            {
                var DichVuList = await _dichVuRepository.SearchDichVu(searchKey);
                if (!DichVuList.Any())
                {
                    return BadRequest("Not found!");
                }
                return Ok(DichVuList);
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
        [Route("/api/[controller]/create-dich-vu")]
        public async Task<ActionResult<string>> CreateDichVu([FromForm] DichVuModel model,  IFormFile imageFile)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<DichVuEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _dichVuRepository.CreateDichVu(mapEntity,imageFile);

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
        [Route("/api/[controller]/update-dich-vu")]
        public async Task<IActionResult> UpdateDichVu(int id,[FromForm] DichVuModel entity, IFormFile? imageFile)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<DichVuEntity>(entity);
                //mapEntity.CreateBy=userId;

                await _dichVuRepository.UpdateDichVu(id, entity,imageFile);

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
        [Route("/api/[controller]/delete-dich-vu")]
        public async Task<ActionResult<DichVuEntity>> DeleteDichVu(int keyId)
        {

            try
            {

                await _dichVuRepository.DeleteDichVu(keyId, false);
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
        [Route("/api/[controller]/get-xuat-dich-vu-by-ma-loai-pdf")]
        public async Task<IActionResult> DownloadPdfFile(int maLoaiDichVu)
        {
            try
            {
                byte[] pdfBytes = await _dichVuRepository.DownloadPdfFile(maLoaiDichVu);

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
