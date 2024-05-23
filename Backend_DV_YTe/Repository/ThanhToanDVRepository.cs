//using AutoMapper;
//using Backend_DV_YTe.Model;
//using Backend_DV_YTe.Repository.Interface;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using System.Security.Cryptography;
//using System.Text;
//using static System.Net.WebRequestMethods;

//namespace Backend_DV_YTe.Repository
//{
//    public class ThanhToanDVRepository : IThanhToanDVRepository
//    {
//        private readonly AppDbContext _context;
//        private readonly IMapper _mapper;
//        private readonly AppSettingModel _appSettings;
       
//        private readonly IConfiguration _configuration;
//        public ThanhToanDVRepository(AppDbContext Context, IMapper mapper, IOptions<AppSettingModel> appSettings, VnPayLibrary vnPayLibrary, IConfiguration configuration)
//        {
//            _context = Context;
//            _mapper = mapper;
//            _appSettings = appSettings.Value;

//            _configuration = configuration;
//        }
//        //public string UrlPayment(double totalAmount, int orderId, IHttpContextAccessor httpContextAccessor)
//        //{
//        //    var urlPayment = "";

//        //    DateTime vietnamDateTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

//        //    string vnp_Returnurl = _configuration["AppSettings:Vnp_Returnurl"];
//        //    string vnp_Url = _configuration["AppSettings:Vnp_Url"];
//        //    string vnp_TmnCode = _configuration["AppSettings:Vnp_TmnCode"];
//        //    string vnp_HashSecret = _configuration["AppSettings:Vnp_HashSecret"];

//        //    VnPayLibrary vnpay = new VnPayLibrary();

//        //    vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
//        //    vnpay.AddRequestData("vnp_Command", "pay");
//        //    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
//        //    vnpay.AddRequestData("vnp_Amount", (totalAmount * 100).ToString());
//        //    vnpay.AddRequestData("vnp_BankCode", "VNBANK");
//        //    vnpay.AddRequestData("vnp_CreateDate", vietnamDateTime.ToString("yyyyMMddHHmmss"));
//        //    vnpay.AddRequestData("vnp_CurrCode", "VND");
//        //    vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContextAccessor));
//        //    vnpay.AddRequestData("vnp_Locale", "vn");
//        //    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
//        //    vnpay.AddRequestData("vnp_OrderType", "other");
//        //    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
//        //    vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());

//        //    urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

//        //    return urlPayment;
//        //}
//        public string UrlPayment(double totalAmount, int orderId, IHttpContextAccessor httpContextAccessor)
//        {
//            var urlPayment = "";

//            DateTime vietnamDateTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

//            string vnp_Returnurl = "https://localhost:7125/api/ThanhToanDV/get-thanh-toan-by-id";
//            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
//            string vnp_TmnCode = "O9PKDYBC";
//            string vnp_HashSecret = "1H5C3ZPS4FO8K94GECHQHT0XC0QYVPRH";

//            VnPayLibrary vnpay = new VnPayLibrary();

//            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
//            vnpay.AddRequestData("vnp_Command", "pay");
//            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
//            vnpay.AddRequestData("vnp_Amount", (totalAmount * 100).ToString());
//            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
//            vnpay.AddRequestData("vnp_CreateDate", vietnamDateTime.ToString("yyyyMMddHHmmss"));
//            vnpay.AddRequestData("vnp_CurrCode", "VND");
//            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContextAccessor));
//            vnpay.AddRequestData("vnp_Locale", "vn");
//            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
//            vnpay.AddRequestData("vnp_OrderType", "other");
//            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
//            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());

//            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

//            return urlPayment;
//        }
//        public static class Utils
//        {
//            public static String HmacSHA512(string key, String inputData)
//            {
//                var hash = new StringBuilder();
//                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
//                byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
//                using (var hmac = new HMACSHA512(keyBytes))
//                {
//                    byte[] hashValue = hmac.ComputeHash(inputBytes);
//                    foreach (var theByte in hashValue)
//                    {
//                        hash.Append(theByte.ToString("x2"));
//                    }
//                }

//                return hash.ToString();
//            }
//            public static string GetIpAddress(IHttpContextAccessor httpContextAccessor)
//            {
//                string ipAddress;
//                try
//                {
//                    var httpContext = httpContextAccessor.HttpContext;
//                    ipAddress = httpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();

//                    if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown") || ipAddress.Length > 45)
//                        ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
//                }
//                catch (Exception ex)
//                {
//                    ipAddress = "Invalid IP: " + ex.Message;
//                }

//                return ipAddress;
//            }
//        }

//        //public async Task<IActionResult> ThanhToanVNPAY()
//        //{
//        //    //int total = 0;
//        //    //var dsDV = await _context.dichVuEntities.ToListAsync(); // Sử dụng phương thức từ Repository API

//        //    //string orderid = DateTime.Now.Ticks.ToString(); 
//        //    //return ;
//        //}

//        //public async Task<IActionResult> string XacNhanThanhToan2()
//        //{
//        //    if (Request.QueryString.Count > 0)
//        //    {
//        //        string vnp_HashSecret = _configuration["AppSettings:Vnp_HashSecret"];
//        //        var vnpayData = Request.QueryString;
//        //        VnPayLibrary vnpay = new VnPayLibrary();

//        //        foreach (string s in vnpayData)
//        //        {
//        //            if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
//        //            {
//        //                vnpay.AddResponseData(s, vnpayData[s]);
//        //            }
//        //        }
//        //        string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
//        //        long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
//        //        string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
//        //        string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
//        //        String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
//        //        String TerminalID = Request.QueryString["vnp_TmnCode"];
//        //        long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
//        //        String bankCode = Request.QueryString["vnp_BankCode"];

//        //        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
//        //    }
//        //    return 0;
//        //}
//    }
//}
