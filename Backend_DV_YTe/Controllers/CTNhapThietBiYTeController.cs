using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;

namespace Backend_DV_YTe.Controllers
{
    //[Authorize(Roles = "NhanVien")]
    [Route("api/[controller]")]
    [ApiController]
    public class CTNhapThietBiYTeController : ControllerBase
    {
        private readonly ICTNhapThietBiYTeRepository _cTNhapThietBiYTeRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTNhapThietBiYTeController(ICTNhapThietBiYTeRepository CTNhapThietBiYTeRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _cTNhapThietBiYTeRepository = CTNhapThietBiYTeRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-ct-nhap-thiet-bi-y-te-by-id")]
        public async Task<ActionResult<CTNhapThietBiYTeEntity>> GetCTNhapThietBiYTeById(int maThuoc, int maNhapThuoc)
        {
            try
            {
                var entity = await _cTNhapThietBiYTeRepository.GetCTNhapThietBiYTeById(maThuoc, maNhapThuoc);

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
        [Route("/api/[controller]/get-all-ct-nhap-thiet-bi-y-te")]
        public async Task<ActionResult<IEnumerable<CTNhapThietBiYTeEntity>>> GetAllCTNhapThietBiYTe()
        {
            try
            {
                var entity = await _cTNhapThietBiYTeRepository.GetAllCTNhapThietBiYTe();

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
        [Route("/api/[controller]/create-ct-nhap-thiet-bi-y-te")]
        public async Task<ActionResult<string>> addCTNhapThietBiYTeAsync (NhapThietBiDto model)
        {
            try
            {
                //byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                //if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                //{
                //    throw new Exception(message: "Vui lòng đăng nhập!");
                //}

                //int userId = BitConverter.ToInt32(userIdBytes, 0);

                //var mapEntity = _mapper.Map<CTNhapThietBiYTeEntity>(model);
                //mapEntity.CreateBy = userId;
                var result = await _cTNhapThietBiYTeRepository.AddNhapThietBiAsync(model);

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
        //[Route("/api/[controller]/update-ct-nhap-thiet-bi-y-te")]
        //public async Task<ActionResult> UpdateCTNhapThietBiYTe(string maVaccine, string maNhapVaccine, CTNhapThietBiYTeModel entity)
        //{
        //    try
        //    {

        //        byte[] userIdBytes = await _distributedCache.GetAsync("UserId"); // Lấy giá trị UserId từ Distributed Cache
        //        string userId = Encoding.UTF8.GetString(userIdBytes);

        //        var mapEntity = _mapper.Map<CTNhapThietBiYTeEntity>(entity);
        //        mapEntity.CreateBy = userId;
        //        await _cTNhapThietBiYTeRepository.UpdateCTNhapThietBiYTe(maVaccine, maNhapVaccine, mapEntity);

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
        [Route("/api/[controller]/get-ct-nhap-thiet-bi-y-te-report")]
        public async Task<ActionResult<Dictionary<int, int>>> GenerateCTXuatThuocReport(DateTime startTime, DateTime endTime)
        {
            try
            {
                var report = _cTNhapThietBiYTeRepository.GenerateNhapThuocReport(startTime, endTime);
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
        [Route("/api/[controller]/delete-ct-nhap-thiet-bi-y-te")]
        public async Task<ActionResult<CTNhapThietBiYTeEntity>> DeleteCTNhapThietBiYTe(int maThuoc, int maNhapThuoc)
        {

            try
            {

                await _cTNhapThietBiYTeRepository.DeleteCTNhapThietBiYTe(maThuoc, maNhapThuoc, false);
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
