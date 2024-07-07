using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ILoThuocRepository
    {
        Task<List<LoThuocEntity>> getLoThuocByMaThuoc(int maThuoc);
        Task<List<LoThuocEntity>> getAllLoThuoc();
        Task CheckAndNotifyExpiringLoThuoc();
        Task<List<string>> GetEmailQL();
        Task<List<LoThuocEntity>> GetExpiringLoThuoc(DateTime expiringDate);
        Task SendEmailAsync(string email, string subject, string message);
        Task HuyLoThuoc(int loThuocId,string lyDoHuy);
        Task<PhieuHuyLoThuocModel> TaoPhieuHuyLoThuoc(int loThuocId, string lyDoHuy);
    }
}
