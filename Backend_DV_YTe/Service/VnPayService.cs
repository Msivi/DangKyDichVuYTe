using Backend_DV_YTe.Model;
using Backend_DV_YTe.Libraries;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_DV_YTe.Entity;
using Org.BouncyCastle.Asn1.X9;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Service
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _distributedCache;
        public VnPayService(IConfiguration configuration, AppDbContext context,IDistributedCache distributedCache)
        {
            _configuration = configuration;
            _context = context;
            _distributedCache = distributedCache;
        }
        public async Task <string> CreatePaymentUrl(int maLichHen, HttpContext context)
        {
            try
            {
                byte[]? userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);
                if (userId < 0)
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                var giaDichVu = (from lh in _context.lichHenEntities
                                 join dv in _context.dichVuEntities on lh.MaDichVu equals dv.Id
                                 where lh.Id == maLichHen && lh.MaDichVu == dv.Id
                                 select dv.gia).FirstOrDefault();

                var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
                var tick = DateTime.Now.Ticks.ToString();
                var pay = new VnPayLibrary();
                var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            
            return response;
        }

        public async Task SavePaymentData(PaymentResponseModel collections)
        {
           


                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var TT = new ThanhToanDVEntity();

                var pay = new VnPayLibrary();
                var orderInfo = collections.OrderDescription.Split(',');
                if (collections.VnPayResponseCode == "00")
                {
                    if (orderInfo.Length >= 2)
                    {
                        if (double.TryParse(orderInfo[0], out double tongTien))
                        {
                            TT.tongTien = tongTien;
                        }
                    }

                    // Tách chuỗi vnp_OrderInfo thành các phần tử
                    if (orderInfo.Length >= 2)
                    {
                        if (int.TryParse(orderInfo[1], out int maLichHen))
                        {
                            TT.MaLichHen = maLichHen;
                        }
                    }

                    TT.ngayThanhToan = DateTime.Now;
                    TT.phuongThucThanhToan = collections.PaymentMethod;
                    TT.trangThai = collections.Success.ToString();
                    TT.CreateBy = userId;

                    await _context.thanhToanDVEntities.AddAsync(TT);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    throw new Exception(message: "Lưu dữ liệu không thanh toán không thành công!");
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
