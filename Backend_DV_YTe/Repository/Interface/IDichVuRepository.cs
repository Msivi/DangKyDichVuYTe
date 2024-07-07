using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IDichVuRepository
    {
        Task<ICollection<DichVuEntity>> GetAllDichVu();
        Task<DichVuEntity> GetDichVuById(int Id);
        Task<ICollection<DichVuEntity>> SearchDichVu(string searchKey);
        Task<string> CreateDichVu(DichVuEntity entity, IFormFile imageFile);
        Task UpdateDichVu(int id, DichVuModel entity, IFormFile? imageFile);
        Task<DichVuEntity> DeleteDichVu(int entity, bool isPhysical);
        Task<ICollection<DichVuEntity>> GetAllDichVuTheoChuyenKhoa(string ChuyenKhoa);
        Task<ICollection<DichVuTheoLoaiModel>> GetDichVuByLoaiDichVuAsync(int? loaiDichVuId, int chuyenKhoaId);
        Task<double> thongKeTinhTongGiaTriDichVu(DateTime fromDate, DateTime toDate);
        //Task<double> TinhGiaTriTrungBinhDichVuTrongKhoangThoiGian(DateTime ngayBD, DateTime ngayKT);
        Task<List<DichVuSuDung>> TimDichVuDaSuDung(DateTime fromDate, DateTime toDate);
        Task<byte[]> DownloadPdfFile(int entity);

    }
}
