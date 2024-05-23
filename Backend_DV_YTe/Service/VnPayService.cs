using Backend_DV_YTe.Model;
using Backend_DV_YTe.Libraries;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Service
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public VnPayService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public string CreatePaymentUrl(int maLichHen, HttpContext context)
        {

            var giaDichVu = (from lh in _context.lichHenEntities
                             join dv in _context.dichVuEntities on lh.MaDichVu equals dv.Id
                             where lh.Id == maLichHen && lh.MaDichVu == dv.Id
                             select dv.gia).FirstOrDefault();

            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            //pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            //pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            //pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            //pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            //pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            //pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            //pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            //pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            //pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            //pay.AddRequestData("vnp_OrderType", model.OrderType);
            //pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            //pay.AddRequestData("vnp_TxnRef", tick);

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)giaDichVu * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{giaDichVu},{maLichHen}");
            pay.AddRequestData("vnp_OrderType", "dich vu y te");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }

        public async Task SavePaymentData(IQueryCollection collections)
        {
            var TT = new ThanhToanDVEntity();

            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            if (response.VnPayResponseCode == "00")
            {
                if (double.TryParse(response.OrderDescription, out double tongTien))
                {
                    TT.tongTien = tongTien;
                }

                // Tách chuỗi vnp_OrderInfo thành các phần tử
                var orderInfo = response.OrderDescription.Split(',');

                if (orderInfo.Length >= 2)
                {
                    if (int.TryParse(orderInfo[1], out int maLichHen))
                    {
                        TT.MaLichHen = maLichHen;
                    }
                }

                TT.ngayThanhToan = DateTime.Now;
                TT.phuongThucThanhToan = response.PaymentMethod;
                TT.trangThai = "true";

                await _context.thanhToanDVEntities.AddAsync(TT);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<double> GetLichHenById(int id)
        {
            var giaDichVu = (from lh in _context.lichHenEntities
                             join dv in _context.dichVuEntities on lh.MaDichVu equals dv.Id
                             where lh.Id == id && lh.MaDichVu == dv.Id
                             select dv.gia).FirstOrDefault();


            return giaDichVu;
        }
    }
}
