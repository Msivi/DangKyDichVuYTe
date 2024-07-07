using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Model.SanPhamHoaDon;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IHoaDonRepository
    {
        Task<ICollection<HoaDonEntity>> GetAllHoaDon();
        Task<ICollection<HoaDonEntity>> GetAllHoaDonKhachHang();
        Task<HoaDonEntity> GetHoaDonById(int id);
        Task<ICollection<SanPhamModel>> GetAllSanPhamByMaHD(int maHD);
        Task<string> CreateHoaDon();
        Task UpdateHoaDon(HoaDonEntity entity);
        Task<HoaDonEntity> DeleteHoaDon(int id, bool isPhysical);
    }
}
