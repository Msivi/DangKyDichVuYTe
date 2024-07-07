using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoThietBiYTeController : ControllerBase
    {
        private readonly ILoThietBiYTeRepository _loThietBiYTeRepository;

        public LoThietBiYTeController(ILoThietBiYTeRepository LoThietBiYTeRepository)
        {
            _loThietBiYTeRepository = LoThietBiYTeRepository;
        }
        [HttpGet]
        [Route("/api/[controller]/get-lo-thiet-bi-y-te-by-ma-thiet-bi")]
        public async Task<ActionResult<LoaiThuocEntity>> GetLoThietBiYTeByMaThietBi(int maThietBi)
        {
            try
            {
                var entity = await _loThietBiYTeRepository.getLoThietBiYTeByMaThietBi(maThietBi);

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
        public async Task<ActionResult<LoaiThuocEntity>> GetAllLoThietBiYTe()
        {
            try
            {
                var entity = await _loThietBiYTeRepository.getAllLoThietBiYTe();

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
        public async Task<IActionResult> HuyLoThietBiYTe(int LoThietBiYTeId, string lyDoHuy)
        {
            try
            {
                var phieuHuy = await _loThietBiYTeRepository.TaoPhieuHuyLoThietBiYTe(LoThietBiYTeId, lyDoHuy);
                return Ok(phieuHuy);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
