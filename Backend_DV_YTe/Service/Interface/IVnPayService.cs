using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
namespace Backend_DV_YTe.Service.Interface
{
    public interface IVnPayService
    {
        //string CreatePaymentUrl(int maLichHen, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
        Task<string> CreatePaymentUrl(int maLichHen, HttpContext context);
        Task<string> CreatePaymentForThuocThietBiUrl(int[] maHoaDon, int maDiaChi, string? ghiChu, HttpContext context);
        Task<string> CreatePaymentUrlMobile(int maLichHen, HttpContext context);
        Task<string> CreatePaymentForThuocThietBiMobile(int[] maHoaDon, int maDiaChi, string? ghiChu, HttpContext context);
        Task<int> SavePaymentData(PaymentResponseModel collections);
        Task<List<int>> SavePaymentDataThuocThietBi(PaymentResponseModel collections);
        Task<double> GetLichHenById(int id);
        Task<string> SendEmailAsync(int maLichHen, string pdfPath);
        Task SendInvoiceEmailAsync(string filePath, string body);


    }
}
