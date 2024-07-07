using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoThuocController : ControllerBase
    {
        private readonly ILoThuocRepository _loThuocRepository;

        public LoThuocController(ILoThuocRepository loThuocRepository)
        {
            _loThuocRepository = loThuocRepository;
        }
        [HttpGet]
        [Route("/api/[controller]/get-lo-thuoc-by-ma-thuoc")]
        public async Task<ActionResult<LoaiThuocEntity>> GetLoThuocByMaThuoc(int maThuoc)
        {
            try
            {
                var entity = await _loThuocRepository.getLoThuocByMaThuoc(maThuoc);

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
        [Route("/api/[controller]/get-all-lo-thuoc")]
        public async Task<ActionResult<LoaiThuocEntity>> GetAllLoThuoc()
        {
            try
            {
                var entity = await _loThuocRepository.getAllLoThuoc();

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
        [Route("/api/[controller]/huy-lo-thuoc")]
        public async Task<IActionResult> HuyLoThuoc(int loThuocId, string lyDoHuy)
        {
            try
            {
                var phieuHuy = await _loThuocRepository.TaoPhieuHuyLoThuoc(loThuocId, lyDoHuy);
                return Ok(phieuHuy);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
