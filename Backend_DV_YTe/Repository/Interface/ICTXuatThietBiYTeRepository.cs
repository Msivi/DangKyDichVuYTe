using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ICTXuatThietBiYTeRepository
    {
        Task<ICollection<CTXuatThietBiYTeEntity>> GetAllCTXuatThietBiYTe();
        Task<CTXuatThietBiYTeEntity> GetCTXuatThietBiYTeById(int maThuoc, int maNhapThuoc);
        //Task<ICollection<CTXuatThietBiYTeEntity>> SearchCTXuatThietBiYTe(string searchKey);
        Task<string> CreateCTXuatThietBiYTe(CTXuatThietBiYTeEntity entity);
        Task UpdateCTXuatThietBiYTe(int maThuoc, int maNhapThuoc, CTXuatThietBiYTeEntity entity);
        Task<CTXuatThietBiYTeEntity> DeleteCTXuatThietBiYTe(int maThuoc, int maNhapThuoc, bool isPhysical);
        Dictionary<int, int> GenerateXuatThuocReport(DateTime startTime, DateTime endTime);

          byte[] DownloadPdfFile();
    }
}
