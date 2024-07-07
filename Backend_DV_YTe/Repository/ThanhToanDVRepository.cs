using AutoMapper;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;
using IOFile = System.IO.File;
using System.Drawing.Printing;
using System.Drawing;
 
using System.IO;
using PdfiumViewer;
using iText.Pdfua;

using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfiumViewer;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using iText.Layout.Element;
using Font = iTextSharp.text.Font;
using Backend_DV_YTe.Entity;
using iText.Kernel.Geom;
using Org.BouncyCastle.Asn1.X509;
using iTextSharp.text.pdf.parser;

namespace Backend_DV_YTe.Repository
{
    public class ThanhToanDVRepository : IThanhToanDVRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettingModel _appSettings;

        private readonly IConfiguration _configuration;
        public ThanhToanDVRepository(AppDbContext Context, IMapper mapper, IOptions<AppSettingModel> appSettings, IConfiguration configuration)
        {
            _context = Context;
            _mapper = mapper;
            _appSettings = appSettings.Value;

            _configuration = configuration;
        }
        public async Task<Invoice> CreateInvoiceAsync(int appointmentId)
        {
            // Lấy thông tin lịch hẹn và thanh toán
            var appointment = await _context.lichHenEntities
                .Include(l => l.KhachHang)
                .ThenInclude(d=>d.DiaChi)
                .Include(l => l.BacSi)
                .Include(l => l.DichVu)
                .Include(l => l.ThanhToanDV)
                .FirstOrDefaultAsync(l => l.Id == appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found");

            var payment = appointment.ThanhToanDV.FirstOrDefault();

            if (payment == null)
                throw new Exception("Payment not found");

            // Tạo hóa đơn
            var locationCustomer = appointment.KhachHang.DiaChi.FirstOrDefault()?.tenDiaChi;
            var invoice = new Invoice
            {
                CustomerName = appointment.KhachHang.tenKhachHang,
                PhoneNumberCustomer= appointment.KhachHang.SDT,
                LocationCustomer= locationCustomer,
                DoctorName = appointment.BacSi.tenBacSi,
                ServiceName = appointment.DichVu.tenDichVu,
                MaDV= appointment.Id,
                Location = appointment.diaDiem,
                AppointmentTime = appointment.thoiGianDuKien,
                TotalAmount = payment.tongTien,
                PaymentDate = payment.ngayThanhToan,
                PaymentMethod = payment.phuongThucThanhToan,
                PaymentStatus = payment.trangThai
            };

            return invoice;
        }
        
        public async Task<string> PrintInvoiceAsync(Invoice invoice)
        {
            var pdfBytes = CreatePdfInvoice(invoice);

            // Lưu file PDF
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            
            var pdfPath = $"D:\\DoAnKySu\\imageDV\\hoaDonDichVu_{timestamp}.pdf";
            System.IO.File.WriteAllBytes(pdfPath, pdfBytes);

            // Gọi lệnh in bằng cách sử dụng GhostScript hoặc một công cụ khác
            PrintPdf(pdfPath);
            return pdfPath;
        }
         
        private void PrintPdf(string pdfPath)
        {
            string fontRelativePath = System.IO.Path.Combine("gswin64c.exe");
            string fontAbsolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fontRelativePath);
             
            var args = $"-dPrinted -dBATCH -dNOPAUSE -sDEVICE=mswinpr2 -sOutputFile=%printer%\"Printer Name\" \"{pdfPath}\"";

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = fontAbsolutePath;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            process.WaitForExit();
        }

        
        public byte[] CreatePdfInvoice(Invoice invoice)
        {
            using (var memoryStream = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 50, 50, 25, 25);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Đường dẫn tới file font Arial Unicode
                string fontRelativePath = System.IO.Path.Combine("arial.ttf");
                string fontAbsolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fontRelativePath);

                // Tạo font hỗ trợ Unicode
                var baseFont = BaseFont.CreateFont(fontAbsolutePath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                var titleFont = new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.BOLD);
                var headerFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                var contentFont = new iTextSharp.text.Font(baseFont, 10);

                // Thêm tiêu đề hóa đơn
                var title = new iTextSharp.text.Paragraph("HÓA ĐƠN THANH TOÁN DỊCH VỤ", titleFont)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_CENTER
                };
                document.Add(title);

                var subTitle = new iTextSharp.text.Paragraph("(Hóa đơn chuyển đổi từ hóa đơn điện tử)", contentFont)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_CENTER
                };
                document.Add(subTitle);

                var dateParagraph = new iTextSharp.text.Paragraph($"Ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}", contentFont)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_CENTER
                };
                document.Add(dateParagraph);

                // Thêm dòng phân cách
                var line = new iTextSharp.text.Paragraph(new iTextSharp.text.Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.BaseColor.BLACK, iTextSharp.text.Element.ALIGN_CENTER, 1)));
                document.Add(line);

                // Thêm thông tin đơn vị bán hàng
                var sellerInfoTable = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f
                };
                sellerInfoTable.SetWidths(new float[] { 1, 3 });

                sellerInfoTable.AddCell(new Phrase("Đơn vị bán hàng:", headerFont));
                sellerInfoTable.AddCell(new Phrase("Công ty cổ phần ABC", contentFont));

                sellerInfoTable.AddCell(new Phrase("Mã số thuế:", headerFont));
                sellerInfoTable.AddCell(new Phrase("0101243150", contentFont));

                sellerInfoTable.AddCell(new Phrase("Địa chỉ:", headerFont));
                sellerInfoTable.AddCell(new Phrase("Tầng 9 Tech,Bình Tân,TP.HCM", contentFont));

                sellerInfoTable.AddCell(new Phrase("Điện thoại:", headerFont));
                sellerInfoTable.AddCell(new Phrase("04 3333 9999", contentFont));

                sellerInfoTable.AddCell(new Phrase("Số tài khoản:", headerFont));
                sellerInfoTable.AddCell(new Phrase("012345678910", contentFont));

                document.Add(sellerInfoTable);

                // Thông tin người mua hàng
                var buyerInfoTable = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f
                };
                buyerInfoTable.SetWidths(new float[] { 1, 3 });

                buyerInfoTable.AddCell(new Phrase("Họ tên người mua hàng:", headerFont));
                buyerInfoTable.AddCell(new Phrase(invoice.CustomerName, contentFont));

                //buyerInfoTable.AddCell(new Phrase("Tên đơn vị:", headerFont));
                //buyerInfoTable.AddCell(new Phrase("Công ty TNHH Bảo Ngọc", contentFont));

                //buyerInfoTable.AddCell(new Phrase("Mã số thuế:", headerFont));
                //buyerInfoTable.AddCell(new Phrase("0101243150", contentFont));

                buyerInfoTable.AddCell(new Phrase("Số điện thoại:", headerFont));
                buyerInfoTable.AddCell(new Phrase(invoice.PhoneNumberCustomer, contentFont));

                //buyerInfoTable.AddCell(new Phrase("Địa chỉ:", headerFont));
                //buyerInfoTable.AddCell(new Phrase(invoice.LocationCustomer, contentFont));

                document.Add(buyerInfoTable);

                // Hình thức thanh toán và thông tin dịch vụ
                var paymentInfoTable = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f
                };
                paymentInfoTable.SetWidths(new float[] { 1, 3 });

                paymentInfoTable.AddCell(new Phrase("Hình thức thanh toán:", headerFont));
                paymentInfoTable.AddCell(new Phrase(invoice.PaymentMethod, contentFont));

                paymentInfoTable.AddCell(new Phrase("Dịch vụ:", headerFont));
                paymentInfoTable.AddCell(new Phrase(invoice.ServiceName, contentFont));

                paymentInfoTable.AddCell(new Phrase("Địa điểm:", headerFont));
                paymentInfoTable.AddCell(new Phrase(invoice.Location, contentFont));

                paymentInfoTable.AddCell(new Phrase("Thời gian:", headerFont));
                paymentInfoTable.AddCell(new Phrase(invoice.AppointmentTime.ToString("g"), contentFont));

                document.Add(paymentInfoTable);

                // Bảng thông tin hàng hóa, dịch vụ
                var serviceTable = new PdfPTable(4)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f
                };
                serviceTable.SetWidths(new float[] { 1, 4, 1, 1 });

                serviceTable.AddCell(new PdfPCell(new Phrase("Mã lịch hẹn", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                serviceTable.AddCell(new PdfPCell(new Phrase("Tên hàng hóa, dịch vụ", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                //serviceTable.AddCell(new PdfPCell(new Phrase("Đơn vị tính", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                serviceTable.AddCell(new PdfPCell(new Phrase("Số lượng", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                serviceTable.AddCell(new PdfPCell(new Phrase("Đơn giá", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                // Add example service item
                serviceTable.AddCell(new PdfPCell(new Phrase(invoice.MaDV.ToString(), contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                serviceTable.AddCell(new PdfPCell(new Phrase(invoice.ServiceName, contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                //serviceTable.AddCell(new PdfPCell(new Phrase("Chiếc", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                serviceTable.AddCell(new PdfPCell(new Phrase("1", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                serviceTable.AddCell(new PdfPCell(new Phrase(invoice.TotalAmount.ToString("C"), contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                document.Add(serviceTable);

                // Tổng tiền
                var totalTable = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f
                };
                totalTable.SetWidths(new float[] { 4, 1 });

                totalTable.AddCell(new Phrase("Cộng tiền hàng:", contentFont));
                totalTable.AddCell(new Phrase(invoice.TotalAmount.ToString("C"), contentFont));

                totalTable.AddCell(new Phrase("Thuế suất GTGT: 0%", contentFont));
                totalTable.AddCell(new Phrase((invoice.TotalAmount ).ToString("C"), contentFont));

                totalTable.AddCell(new Phrase("Tổng tiền thanh toán:", contentFont));
                totalTable.AddCell(new Phrase((invoice.TotalAmount).ToString("C"), contentFont));

                //totalTable.AddCell(new Phrase("Số tiền viết bằng chữ:", contentFont));
                //totalTable.AddCell(new Phrase("Tám triệu tám trăm nghìn đồng chẵn", contentFont)); // Example text

                document.Add(totalTable);

                // Chữ ký
                var signatureTable = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 30f,
                    SpacingAfter = 10f
                };
                signatureTable.SetWidths(new float[] { 1, 1 });

                //signatureTable.AddCell(new PdfPCell(new Phrase("Người chuyển đổi", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER });
                signatureTable.AddCell(new PdfPCell(new Phrase("Người mua hàng", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER });
                signatureTable.AddCell(new PdfPCell(new Phrase("Người bán hàng", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER });

                //signatureTable.AddCell(new PdfPCell(new Phrase("(Ký, ghi rõ họ tên)", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER });
                signatureTable.AddCell(new PdfPCell(new Phrase("(Ký, ghi rõ họ tên)", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER });
                signatureTable.AddCell(new PdfPCell(new Phrase("(Ký, ghi rõ họ tên)", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER });

                //signatureTable.AddCell(new PdfPCell(new Phrase("Lê Thanh Nam", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER, PaddingTop = 40 });
                signatureTable.AddCell(new PdfPCell(new Phrase(invoice.CustomerName, contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER, PaddingTop = 40 });
                signatureTable.AddCell(new PdfPCell(new Phrase("Ký bởi: CÔNG TY CỔ PHẦN ABC", contentFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = iTextSharp.text.Rectangle.NO_BORDER, PaddingTop = 40 });

                document.Add(signatureTable);

                document.Close();
                writer.Close();

                return memoryStream.ToArray();
            }
        }
        public async Task<ThanhToanDVEntity> GetTTThanhToan(int appointmentId)
        {
            // Lấy thông tin lịch hẹn và thanh toán
            var appointment = await _context.thanhToanDVEntities
                .FirstOrDefaultAsync(l => l.MaLichHen == appointmentId && l.DeletedTime==null);

            if (appointment == null)
                throw new Exception("Appointment not found");

            

            return appointment;
        }

    }
}
