using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanDVController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _distributedCache;
        private readonly IThanhToanDVRepository _thanhToanDVRepository;
        public ThanhToanDVController(IVnPayService vnPayService,AppDbContext appContext, IDistributedCache distributedCache, IThanhToanDVRepository thanhToanDVRepository)
        {
            _vnPayService = vnPayService;
            _context = appContext;
            _distributedCache = distributedCache;
            _thanhToanDVRepository = thanhToanDVRepository;
        }

        [HttpPost]
        [Route("/api/[controller]/thanh-toan-by-id")]
        public  async Task< IActionResult> CreatePaymentUrl(int maLichHen)
        {
            try
            {
                var existingLichHen = _context.thanhToanDVEntities.FirstOrDefault(c => c.MaLichHen == maLichHen && c.DeletedTime == null);
                
                if(existingLichHen!=null){
                    throw new Exception(message: "Dịch vụ này đã được thanh toán");
                }
                
                var url =await _vnPayService.CreatePaymentUrl(maLichHen, HttpContext);

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
        [Route("/api/[controller]/thanh-toan-mobile-by-id")]
        public async Task<IActionResult> CreatePaymentUrlMobile(int maLichHen)
        {
            try
            {
                var existingLichHen = _context.thanhToanDVEntities.FirstOrDefault(c => c.MaLichHen == maLichHen && c.DeletedTime == null);

                if (existingLichHen != null)
                {
                    throw new Exception(message: "Dịch vụ này đã được thanh toán");
                }

                var url = await _vnPayService.CreatePaymentUrlMobile(maLichHen, HttpContext);

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

        [HttpGet]
        [Route("/api/[controller]/thanh-toan-by-id")]
        public async Task<IActionResult> PaymentCallbackAsync()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode != "00" || !response.Success)
            {
                return Redirect($"http://localhost:3000/admin/lichhen/payment_failed");
            }
            var saveThanhToan = await _vnPayService.SavePaymentData(response);
           

            var inHoaDon = await _thanhToanDVRepository.CreateInvoiceAsync(saveThanhToan);

            var pdfPath= await _thanhToanDVRepository.PrintInvoiceAsync(inHoaDon);
            
            var guiEmail = await _vnPayService.SendEmailAsync(saveThanhToan, pdfPath);

            //====
            if (response.VnPayResponseCode == "00" && response.Success)
            {
                 
                var lichHen = await _context.lichHenEntities.FirstOrDefaultAsync(lh => lh.Id == saveThanhToan);
                return Redirect($"http://localhost:3000/admin/lichhen/{lichHen.Id}");
            }
            else
            {
                return Redirect($"http://localhost:3000/admin/lichhen/payment_failed");
            }
         
        }
        [HttpGet]
        [Route("/api/[controller]/thanh-toan-mobile-by-id")]
        public async Task<IActionResult> PaymentCallbackMobileAsync()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode != "00" || !response.Success)
            {
                return Ok("Thanh toán không thành công");
            }
            var saveThanhToan = await _vnPayService.SavePaymentData(response);
              
            var inHoaDon = await _thanhToanDVRepository.CreateInvoiceAsync(saveThanhToan);

            var pdfPath= await _thanhToanDVRepository.PrintInvoiceAsync(inHoaDon);

            var guiEmail = await _vnPayService.SendEmailAsync(saveThanhToan,pdfPath);

            return  Ok(response);

        }

        //[HttpGet]
        //[Route("/api/[controller]/thanh-toan-by-id")]
        //public async Task<IActionResult> PaymentCallbackAsync(int maLichHen)
        //{
        //    try
        //    {
        //        var response = _vnPayService.PaymentExecute(Request.Query);
        //        if (!response.Success)
        //        {
        //            // Trả về trang lỗi nếu thanh toán không thành công
        //            return Redirect("https://localhost:7125/api/ThanhToanDV/thanh-toan-by-id/payment-failure"); // Thay bằng trang lỗi của bạn
        //        }

        //        var saveThanhToan = await _vnPayService.SavePaymentData(response);

        //        // Cập nhật trạng thái thanh toán của lịch hẹn
        //        var lichHen = await _context.lichHenEntities.FirstOrDefaultAsync(lh => lh.Id == saveThanhToan);
        //        if (lichHen != null)
        //        {
        //            lichHen.trangThai = "Paid"; // Cập nhật trạng thái thanh toán của lịch hẹn
        //            await _context.SaveChangesAsync();
        //        }

        //        var guiEmail = await _vnPayService.SendEmailAsync(saveThanhToan);
        //        var inHoaDon = await _thanhToanDVRepository.CreateInvoiceAsync(saveThanhToan);
        //        await _thanhToanDVRepository.PrintInvoiceAsync(inHoaDon);

        //        // Chuyển hướng đến trang lịch hẹn với trạng thái cập nhật
        //        return Redirect($"https://localhost:7125/api/ThanhToanDV/thanh-toan-by-id?maLichHen={lichHen.Id}"); // Thay bằng trang lịch hẹn của bạn
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
        [Route("/api/[controller]/get-thanh-toan-by-id-lich-hen")]
        public async Task<IActionResult> getTTThanhToan(int maLichHen)
        {
            try
            {
                var response = await _thanhToanDVRepository.GetTTThanhToan(maLichHen);
                return Ok(response);
            }catch (Exception ex)
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
