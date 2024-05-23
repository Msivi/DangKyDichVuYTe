using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
namespace Backend_DV_YTe.Service.Interface
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(int maLichHen, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

        Task SavePaymentData(IQueryCollection collections);
        Task<double> GetLichHenById(int id);
    }
}
