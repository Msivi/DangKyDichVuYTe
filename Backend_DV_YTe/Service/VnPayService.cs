using Backend_DV_YTe.Model;
using Backend_DV_YTe.Libraries;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_DV_YTe.Entity;
using Org.BouncyCastle.Asn1.X9;
using Microsoft.Extensions.Caching.Distributed;
using Backend_DV_YTe.Repository.Interface;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace Backend_DV_YTe.Service
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _distributedCache;
        private readonly IHoaDonRepository _hoaDonRepository;
        public VnPayService(IConfiguration configuration, AppDbContext context,IDistributedCache distributedCache,IHoaDonRepository hoaDonRepository)
        {
            _configuration = configuration;
            _context = context;
            _distributedCache = distributedCache;
            _hoaDonRepository = hoaDonRepository;
        }
        public async Task <string> CreatePaymentUrl(int maLichHen, HttpContext context)
        {
            try
            {
                 

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);
 
                var giaDichVu = (from lh in _context.lichHenEntities
                                 join dv in _context.dichVuEntities on lh.MaDichVu equals dv.Id
                                 where lh.Id == maLichHen && lh.MaDichVu == dv.Id
                                 select dv.gia).FirstOrDefault();


                var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
                var tick = DateTime.Now.Ticks.ToString();
                var pay = new VnPayLibrary();
                var urlCallBack = _configuration["PaymentCallBack:ReturnUrlDV"];

                pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
                pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
                pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
                pay.AddRequestData("vnp_Amount", ((int)giaDichVu)+"00".ToString());
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
        // thanh toán cho thuốc, thiết bị
        public async Task<string> CreatePaymentForThuocThietBiUrl(int[] maHoaDon, int maDiaChi, string? ghiChu, HttpContext context)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }
                int userId = BitConverter.ToInt32(userIdBytes, 0);


                var result = await _context.hoaDonEntities
                             .Where(c => maHoaDon.Contains(c.Id) && c.DeletedTime == null)
                             .Select(c => new { c.tongTien, c.Id })
                             .ToListAsync();
                var tongTien = result.Sum(c => c.tongTien);
                var danhSachId = result.Select(c => c.Id).ToList();
                var danhSachIdString = string.Join(",", danhSachId);

                var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
                var tick = DateTime.Now.Ticks.ToString();
                var pay = new VnPayLibrary();
                var urlCallBack = _configuration["PaymentCallBack:ReturnUrlTB"];


                pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
                pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
                pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
                pay.AddRequestData("vnp_Amount", ((int)tongTien * 100).ToString());
                pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
                pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
                pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
                pay.AddRequestData("vnp_OrderInfo", $"{tongTien},{maDiaChi},{ghiChu},{danhSachIdString}");
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
        public async Task<string> CreatePaymentUrlMobile(int maLichHen, HttpContext context)
        {
            try
            {


                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }

                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var giaDichVu = (from lh in _context.lichHenEntities
                                 join dv in _context.dichVuEntities on lh.MaDichVu equals dv.Id
                                 where lh.Id == maLichHen && lh.MaDichVu == dv.Id
                                 select dv.gia).FirstOrDefault();


                var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
                var tick = DateTime.Now.Ticks.ToString();
                var pay = new VnPayLibrary();
                var urlCallBack = _configuration["PaymentCallBack:ReturnUrlDVMobile"];

                pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
                pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
                pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
                pay.AddRequestData("vnp_Amount", ((int)giaDichVu) + "00".ToString());
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
        public async Task<string> CreatePaymentForThuocThietBiMobile (int[] maHoaDon, int maDiaChi, string? ghiChu, HttpContext context)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                if (userIdBytes == null || userIdBytes.Length != sizeof(int))
                {
                    throw new Exception(message: "Vui lòng đăng nhập!");
                }
                int userId = BitConverter.ToInt32(userIdBytes, 0);


                var result = await _context.hoaDonEntities
                             .Where(c => maHoaDon.Contains(c.Id) && c.DeletedTime == null)
                             .Select(c => new { c.tongTien, c.Id })
                             .ToListAsync();
                var tongTien = result.Sum(c => c.tongTien);
                var danhSachId = result.Select(c => c.Id).ToList();
                var danhSachIdString = string.Join(",", danhSachId);

                var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
                var tick = DateTime.Now.Ticks.ToString();
                var pay = new VnPayLibrary();
                var urlCallBack = _configuration["PaymentCallBack:ReturnUrlTBMoBile"];


                pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
                pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
                pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
                pay.AddRequestData("vnp_Amount", ((int)tongTien * 100).ToString());
                pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
                pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
                pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
                pay.AddRequestData("vnp_OrderInfo", $"{tongTien},{maDiaChi},{ghiChu},{danhSachIdString}");
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

        
        public async Task<int> SavePaymentData(PaymentResponseModel collections)
        {
            // Check if user is logged in
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception("Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            // Create a new ThanhToanDV entity
            var thanhToan = new ThanhToanDVEntity();

            // Process payment data only if payment was successful (VnPayResponseCode == "00")
            if (collections.VnPayResponseCode == "00" && collections.Success)
            {
                // Extract total amount
                string[] orderInfo = collections.OrderDescription.Split(',');
                if (orderInfo.Length >= 2 && double.TryParse(orderInfo[0], out double tongTien))
                {
                    thanhToan.tongTien = tongTien;
                }

                // Extract appointment ID
                if (orderInfo.Length >= 2 && int.TryParse(orderInfo[1], out int maLichHen))
                {
                    thanhToan.MaLichHen = maLichHen;
                }
                //if (orderInfo.Length >= 2)
                //{
                //    string ghiChu = orderInfo[2];
                //    thanhToan.ghiChu = ghiChu;
                //}

                // Set other payment details
                thanhToan.ngayThanhToan = DateTime.Now;
                thanhToan.phuongThucThanhToan = collections.PaymentMethod;
                thanhToan.trangThai = collections.Success.ToString();
                thanhToan.CreateBy = userId;
 
                // Save payment data
                await _context.thanhToanDVEntities.AddAsync(thanhToan);
                await _context.SaveChangesAsync();

                return thanhToan.MaLichHen; // Return appointment ID
            }
            else
            {
                 
                throw new Exception(message:"Thanh toán không thành công!"); // More specific message?
            }
        }


        // save thoong tin của thuốc, thiets bị đã mua
        public async Task<List<int>> SavePaymentDataThuocThietBi(PaymentResponseModel collections)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }
            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var pay = new VnPayLibrary();
            var orderInfo = collections.OrderDescription.Split(',');
            if (collections.VnPayResponseCode == "00")
            {
                if (orderInfo.Length >= 2)
                {
                    if (double.TryParse(orderInfo[0], out double tongTien))
                    {
                        var dc = orderInfo[1];
                        var ghiChu = orderInfo[2];
                        var danhSachId = orderInfo.Skip(3).Select(int.Parse).ToList();

                        foreach (var id in danhSachId)
                        {
                            var hoaDon = await _hoaDonRepository.GetHoaDonById(id);

                            if (hoaDon != null)
                            {
                                hoaDon.ngayMua = DateTime.Now;
                                hoaDon.trangThai = collections.Success.ToString();
                                hoaDon.diaChi = dc;
                                hoaDon.ghiChu = ghiChu;
                               await _hoaDonRepository.UpdateHoaDon(hoaDon);
                            }
                        }

                        await _context.SaveChangesAsync();
                        return danhSachId;
                    }
                }
            }
            else
            {
                throw new Exception(message: "Lưu dữ liệu không thanh toán không thành công!");
            }

            // Trả về danh sách rỗng nếu có lỗi không mong muốn xảy ra (có thể thay đổi tuỳ theo yêu cầu của bạn)
            return new List<int>();
        }

        public async Task<double> GetLichHenById(int id)
        {
            var giaDichVu = (from lh in _context.lichHenEntities
                             join dv in _context.dichVuEntities on lh.MaDichVu equals dv.Id
                             where lh.Id == id && lh.MaDichVu == dv.Id
                             select dv.gia).FirstOrDefault();


            return giaDichVu;
        }

        //public async Task<string> SendEmailAsync(int maLichHen)
        //{
        //    byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
        //    if (userIdBytes == null || userIdBytes.Length != sizeof(int))
        //    {
        //        throw new Exception("Vui lòng đăng nhập!");
        //    }

        //    int userId = BitConverter.ToInt32(userIdBytes, 0);

        //    var existingEmail = await _context.khachHangEntities
        //                                      .Where(c => c.maKhachHang == userId)
        //                                      .Select(c => new { email = c.email, tenKh = c.tenKhachHang })
        //                                      .FirstOrDefaultAsync();

        //    if (existingEmail == null)
        //    {
        //        throw new Exception("Không tìm thấy thông tin khách hàng.");
        //    }

        //    var existingLichHen = await _context.lichHenEntities
        //                                        .Include(d => d.DichVu)
        //                                        .FirstOrDefaultAsync(c => c.Id == maLichHen);

        //    if (existingLichHen == null)
        //    {
        //        throw new Exception("Không tìm thấy lịch hẹn.");
        //    }

        //    string body = $"Xin chào! {existingEmail.tenKh}<br>" +
        //                  "Cảm ơn bạn đã tin tưởng chúng tôi.<br>" +
        //                  "Sau đây chúng tôi xin thông báo lịch hẹn khám bệnh mà bạn đã đặt trên hệ thống của chúng tôi<br>" +
        //                  $"Tên dịch vụ: {existingLichHen.DichVu.tenDichVu}<br>" +
        //                  $"Địa điểm: {existingLichHen.diaDiem}<br>" +
        //                  $"Thời gian: {existingLichHen.thoiGianDuKien}<br>";

        //    var email = new MimeMessage();
        //    email.From.Add(MailboxAddress.Parse("july6267@gmail.com"));
        //    email.To.Add(MailboxAddress.Parse(existingEmail.email));
        //    email.Subject = "Thông tin lịch hẹn";
        //    email.Body = new TextPart(TextFormat.Html) { Text = body };

        //    using (var client = new SmtpClient())
        //    {
        //        try
        //        {
        //            client.Connect("smtp.gmail.com", 587, false);
        //            client.Authenticate("july6267@gmail.com", "tape xhgh khov qusg");
        //            client.Send(email);
        //            client.Disconnect(true);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Gửi email không thành công: " + ex.Message);
        //        }
        //    }

        //    return "Email đã được gửi thành công!";
        //}
        public async Task<string> SendEmailAsync(int maLichHen, string pdfPath)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception("Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var existingEmail = await _context.khachHangEntities
                                              .Where(c => c.maKhachHang == userId)
                                              .Select(c => new { email = c.email, tenKh = c.tenKhachHang })
                                              .FirstOrDefaultAsync();

            if (existingEmail == null)
            {
                throw new Exception("Không tìm thấy thông tin khách hàng.");
            }

            var existingLichHen = await _context.lichHenEntities
                                                .Include(d => d.DichVu)
                                                .FirstOrDefaultAsync(c => c.Id == maLichHen);

            if (existingLichHen == null)
            {
                throw new Exception("Không tìm thấy lịch hẹn.");
            }

            string body = $"Xin chào! {existingEmail.tenKh}<br>" +
                          "Cảm ơn bạn đã tin tưởng chúng tôi.<br>" +
                          "Sau đây chúng tôi xin thông báo lịch hẹn khám bệnh mà bạn đã đặt trên hệ thống của chúng tôi<br>" +
                          $"Tên dịch vụ: {existingLichHen.DichVu.tenDichVu}<br>" +
                          $"Địa điểm: {existingLichHen.diaDiem}<br>" +
                          $"Thời gian: {existingLichHen.thoiGianDuKien}<br>";

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("july6267@gmail.com"));
            email.To.Add(MailboxAddress.Parse(existingEmail.email));
            email.Subject = "Thông tin lịch hẹn";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            // Đính kèm file PDF
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(File.OpenRead(pdfPath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(pdfPath)
            };
            var multipart = new Multipart("mixed");
            multipart.Add(email.Body);
            multipart.Add(attachment);
            email.Body = multipart;

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("july6267@gmail.com", "tape xhgh khov qusg");
                    client.Send(email);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Gửi email không thành công: " + ex.Message);
                }
            }

            return "Email đã được gửi thành công!";
        }



        public async Task SendInvoiceEmailAsync(string filePath, string body)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception(message: "Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);

            var emaiKhachHang = await _context.khachHangEntities.FirstOrDefaultAsync(c => c.maKhachHang == userId);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("july6267@gmail.com"));
            email.To.Add(MailboxAddress.Parse(emaiKhachHang.email));
            email.Subject = "Thông tin lịch hẹn";

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            bodyBuilder.Attachments.Add(filePath);

            email.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("july6267@gmail.com", "tape xhgh khov qusg");
                    await client.SendAsync(email);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Gửi email không thành công: " + ex.Message);
                }
            }
        }

    }
}
