using Backend_DV_YTe.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ICTXuatThuocRepository
    {
        Task<ICollection<CTXuatThuocEntity>> GetAllCTXuatThuoc();
        Task<CTXuatThuocEntity> GetCTXuatThuocById(int maThuoc, int maNhapThuoc);
        //Task<ICollection<CTXuatThuocEntity>> SearchCTXuatThuoc(string searchKey);
        Task<string> CreateCTXuatThuoc(CTXuatThuocEntity entity);
        Task UpdateCTXuatThuoc(int maThuoc, int maNhapThuoc, CTXuatThuocEntity entity);
        Task<CTXuatThuocEntity> DeleteCTXuatThuoc(int maThuoc, int maNhapThuoc, bool isPhysical);
        Dictionary<int, int> GenerateXuatThuocReport(DateTime startTime, DateTime endTime);
        byte[] DownloadPdfFile();

    }
}
