using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanDVController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        public ThanhToanDVController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost]
        [Route("/api/[controller]/thanh-toan-by-id")]
        public IActionResult CreatePaymentUrl(int maLichHen)
        {
            try
            {
                var url = _vnPayService.CreatePaymentUrl(maLichHen, HttpContext);

                //return RedirectPermanent(url);
                // Hiển thị đường dẫn URL trong phản hồi
                return Ok(new { Url = url });
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
        [Route("/api/[controller]/luu-thanh-toan")]
        public IActionResult SavePaymetData()
        {
            try
            {
                var url = _vnPayService.SavePaymentData(Request.Query);

                //return RedirectPermanent(url);
                // Hiển thị đường dẫn URL trong phản hồi
                return Ok("Thanh toán thành công!");
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
        [Route("/api/[controller]/thanh-toan-by-id")]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Ok(response);
        }

        [HttpGet]
        [Route("/api/[controller]/get-by-id")]
        public async Task<ActionResult<ThietBiYTeEntity>> GetThietBiYTeById(int id)
        {
            try
            {
                var entity = await _vnPayService.GetLichHenById(id);

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
    }
}
