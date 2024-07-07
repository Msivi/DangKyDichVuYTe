using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IKetQuaDichVuRepository
    {
        Task<ICollection<KetQuaDichVuEntity>> GetAllKetQuaDichVu();
        Task<KetQuaDichVuEntity> GetKetQuaDichVuById(int Id);
        //Task<ICollection<KetQuaDichVuEntity>> SearchKetQuaDichVu(string searchKey);
        Task<string> CreateKetQuaDichVu(KetQuaDichVuEntity entity);
        Task UpdateKetQuaDichVu(int id, KetQuaDichVuModel entity);
        Task<KetQuaDichVuEntity> DeleteKetQuaDichVu(int entity, bool isPhysical);

        //Task<ICollection<KetQuaDichVuEntity>> GetKetQuaDichVuByKhachHang();
        Task<ICollection<TTKetQuaDichVuKhachHangModel>> GetKetQuaDichVuByKhachHang();
        Task<ICollection<KetQuaDichVuEntity>> GetKetQuaDichVuByBacSi();
        Task<List<ThongKeDichVuModel>> GetAverageRatingsByServiceAsync(DateTime startDate, DateTime endDate);
    }
}
