using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ILoaiThuocRepository
    {
        Task<ICollection<LoaiThuocEntity>> GetAllLoaiThuoc();
        Task<LoaiThuocEntity> GetLoaiThuocById(int Id);
        //Task<ICollection<LoaiThuocEntity>> SearchLoaiThuoc(string searchKey);
        Task<string> CreateLoaiThuoc(LoaiThuocEntity entity);
        Task UpdateLoaiThuoc(int id, LoaiThuocEntity entity);
        Task<LoaiThuocEntity> DeleteLoaiThuoc(int entity, bool isPhysical);

        void SendEmail(string recipientEmail, string subject, string message);
    }
}
