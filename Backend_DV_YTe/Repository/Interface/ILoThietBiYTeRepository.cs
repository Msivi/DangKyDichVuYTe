using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ILoThietBiYTeRepository
    {
        Task<List<LoThietBiYTeEntity>> getLoThietBiYTeByMaThietBi(int maThuoc);
        Task<List<LoThietBiYTeEntity>> getAllLoThietBiYTe();
        Task CheckAndNotifyExpiringLoThietBiYTe();
        Task<List<string>> GetEmailQL();
        Task<List<LoThietBiYTeEntity>> GetExpiringLoThietBiYTe(DateTime expiringDate);
        Task SendEmailAsync(string email, string subject, string message);
        Task HuyLoThietBiYTe(int LoThietBiYTeId, string lyDoHuy);
        Task<PhieuHuyLoThietBiYTeModel> TaoPhieuHuyLoThietBiYTe(int LoThietBiYTeId, string lyDoHuy);
    }
}
