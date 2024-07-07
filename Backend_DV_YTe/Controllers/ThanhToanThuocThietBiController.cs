using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanThuocThietBiController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IThietBiYteRepository _thietBiYteRepository;
        private readonly IDistributedCache _distributedCache;
        public ThanhToanThuocThietBiController(IVnPayService vnPayService, IThietBiYteRepository thietBiYteRepository, IDistributedCache distributedCache)
        {
            _vnPayService = vnPayService;
            _thietBiYteRepository = thietBiYteRepository;
            _distributedCache = distributedCache;
        }

        [HttpPost]
        [Route("/api/[controller]/thanh-toan-by-id-hoa-don")]
        public IActionResult CreatePaymentUrl(int[] maHoaDon,string? ghiChu, int maDiaChi)
        {
            try
            {
                var url = _vnPayService.CreatePaymentForThuocThietBiUrl(maHoaDon,maDiaChi,ghiChu, HttpContext);

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
        [Route("/api/[controller]/thanh-toan-by-id-hoa-don-mobile")]
        public IActionResult CreatePaymentUrlMobile(int[] maHoaDon, string? ghiChu, int maDiaChi)
        {
            try
            {
                var url = _vnPayService.CreatePaymentForThuocThietBiMobile(maHoaDon, maDiaChi, ghiChu, HttpContext);

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
        //[HttpPost]
        //[Route("/api/[controller]/luu-thanh-toan-thuoc-thiet-bi")]
        //public IActionResult SavePaymetData(PaymentResponseModel t)
        //{
        //    try
        //    {


        //        var url = _vnPayService.SavePaymentData(t);

        //        //return RedirectPermanent(url);
        //        // Hiển thị đường dẫn URL trong phản hồi
        //        return Ok("Thanh toán thành công!");
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
        [HttpGet]
        [Route("/api/[controller]/luu-thanh-toan-thiet-bi-thuoc")]
        public async Task <IActionResult> PaymentCallback()
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);
            
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode != "00" || !response.Success)
            {
                return Redirect($"http://localhost:3000/admin/lichhen/payment_failed");
            }
            var saveDB =await _vnPayService.SavePaymentDataThuocThietBi(response);
            // cập nhât số lượng đã bán
            var updateSL = await _thietBiYteRepository.CapNhatSoLuongDaBan(saveDB);
            // Tạo và in hóa đơn
            var pdfBytes = await _thietBiYteRepository.CreatePdfInvoices(saveDB);
            var pdfPath = Path.Combine(Path.GetTempPath(), $"invoices_{Guid.NewGuid()}.pdf");
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);
            _thietBiYteRepository.PrintPdf(pdfPath);
            // send email
            string emailBody = "Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi. Đính kèm là hóa đơn của bạn.";
             
            await _vnPayService.SendInvoiceEmailAsync(pdfPath, emailBody);

            //if (response.VnPayResponseCode == "00" && response.Success)
            //{
            //    return Redirect($"http://localhost:3000/admin/lichhen/success");
            //}
            return Ok(response);
        }

        [HttpGet]
        [Route("/api/[controller]/luu-thanh-toan-thiet-bi-thuoc-moblie")]
        public async Task<IActionResult> PaymentCallbackMobile()
        {
            var response =  _vnPayService.PaymentExecute(Request.Query);
            var saveDB = await _vnPayService.SavePaymentDataThuocThietBi(response);
            // cập nhât số lượng đã bán
            if (response.VnPayResponseCode == "00" || response.Success)
            {
                var updateSL = await _thietBiYteRepository.CapNhatSoLuongDaBan(saveDB);
                // Tạo và in hóa đơn
                var pdfBytes = await _thietBiYteRepository.CreatePdfInvoices(saveDB);
                var pdfPath = Path.Combine(Path.GetTempPath(), $"invoices_{Guid.NewGuid()}.pdf");
                await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);
                _thietBiYteRepository.PrintPdf(pdfPath);
                // send email
                string emailBody = "Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi. Đính kèm là hóa đơn của bạn.";

                await _vnPayService.SendInvoiceEmailAsync(pdfPath, emailBody);
            }
            
            return Ok(response);
        }
    }
}
