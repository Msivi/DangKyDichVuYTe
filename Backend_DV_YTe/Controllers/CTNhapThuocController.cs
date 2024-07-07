using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CTNhapThuocController : ControllerBase
    {
        private readonly ICTNhapThuocRepository _cTNhapThuocRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTNhapThuocController(ICTNhapThuocRepository CTNhapThuocRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _cTNhapThuocRepository = CTNhapThuocRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-ct-nhap-thuoc-by-id")]
        public async Task<ActionResult<CTNhapThuocEntity>> GetCTNhapThuocById(int maThuoc, int maNhapThuoc)
        {
            try
            {
                var entity = await _cTNhapThuocRepository.GetCTNhapThuocById(maThuoc, maNhapThuoc);

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
        [Route("/api/[controller]/get-all-ct-nhap-thuoc")]
        public async Task<ActionResult<IEnumerable<CTNhapThuocEntity>>> GetAllCTNhapThuoc()
        {
            try
            {
                var entity = await _cTNhapThuocRepository.GetAllCTNhapThuoc();

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
        [Route("/api/[controller]/create-ct-nhap-thuoc")]
        public async Task<ActionResult<string>> CreateCTNhapThuoc(CTNhapThuocModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<CTNhapThuocEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _cTNhapThuocRepository.CreateCTNhapThuoc(mapEntity);

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
        [HttpPost]
        [Route("/api/[controller]/add-ct-nhap-thuoc-asyns")]
        public async Task<ActionResult<string>> AddCTNhapThuocAsync(NhapThuocDto nhapThuocDto)
        {
            try
            {
                 

               
                var result = await _cTNhapThuocRepository.AddNhapThuocAsync(nhapThuocDto);

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
        //[HttpPut]
        //[Route("/api/[controller]/update-ct-nhap-thuoc")]
        //public async Task<ActionResult> UpdateCTNhapThuoc(string maVaccine, string maNhapVaccine, CTNhapThuocModel entity)
        //{
        //    try
        //    {

        //        byte[] userIdBytes = await _distributedCache.GetAsync("UserId"); // Lấy giá trị UserId từ Distributed Cache
        //        string userId = Encoding.UTF8.GetString(userIdBytes);

        //        var mapEntity = _mapper.Map<CTNhapThuocEntity>(entity);
        //        mapEntity.CreateBy = userId;
        //        await _cTNhapThuocRepository.UpdateCTNhapThuoc(maVaccine, maNhapVaccine, mapEntity);

        //        return Ok(new BaseResponseModel<string>(
        //            statusCode: StatusCodes.Status200OK,
        //            code: "Success!",
        //            data: "Chi tiết nhập vaccine updated successfully!"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BaseResponseModel<string>(
        //            statusCode: StatusCodes.Status400BadRequest,
        //            code: "Error",
        //            message: ex.Message));
        //    }
        //}
        [HttpGet]
        [Route("/api/[controller]/get-ct-nhap-thuoc-report")]
        public async Task<ActionResult<Dictionary<int, int>>> GenerateCTXuatThuocReport(DateTime startTime, DateTime endTime)
        {
            try
            {
                var report = _cTNhapThuocRepository.GenerateNhapThuocReport(startTime, endTime);
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
        [HttpDelete]
        [Route("/api/[controller]/delete-ct-nhap-thuoc")]
        public async Task<ActionResult<CTNhapThuocEntity>> DeleteCTNhapThuoc(int maThuoc, int maNhapThuoc)
        {

            try
            {

                await _cTNhapThuocRepository.DeleteCTNhapThuoc(maThuoc, maNhapThuoc, false);
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
