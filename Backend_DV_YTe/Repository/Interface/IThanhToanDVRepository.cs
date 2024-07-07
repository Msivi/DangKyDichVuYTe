using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IThanhToanDVRepository
    {
         
        Task<Invoice> CreateInvoiceAsync(int appointmentId);
        Task<string> PrintInvoiceAsync(Invoice invoice);
        byte[] CreatePdfInvoice(Invoice invoice);
        Task<ThanhToanDVEntity> GetTTThanhToan(int appointmentId);
    }
}
