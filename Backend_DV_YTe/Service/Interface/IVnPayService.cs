using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
namespace Backend_DV_YTe.Service.Interface
{
    public interface IVnPayService
    {
        //string CreatePaymentUrl(int maLichHen, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
        Task<string> CreatePaymentUrl(int maLichHen, HttpContext context);
        Task SavePaymentData(PaymentResponseModel collections);
        Task<double> GetLichHenById(int id);
    }
}
