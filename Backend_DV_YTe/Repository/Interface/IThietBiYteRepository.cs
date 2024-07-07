using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface IThietBiYteRepository
    {
        Task<ICollection<AllThietBiModel>> GetAllThietBiYTe();
        Task<AllThietBiModel> GetThietBiYTeById(int id);
        Task<ICollection<AllThietBiModel>> SearchThietBiYTe(string searchKey);
        Task<string> CreateThietBiYTe(ThietBiYTeEntity entity, IFormFile? imageFile);
        Task UpdateThietBiYTe(int id, ThietBiYTeEntity entity, IFormFile? imageFile);
        Task<ThietBiYTeEntity> DeleteThietBiYTe(int entity, bool isPhysical);
        Task<ICollection<AllThietBiModel>> GetAllThietBiByMaLoaiTB(int maL);
        Task<byte[]> DownloadPdfFile(int entity);
        Task<byte[]> CreatePdfInvoices(List<int> maHDList);
        void PrintPdf(string pdfPath);
        Task PrintPdfs(List<byte[]> pdfBytesList);
        Task<string> CapNhatSoLuongDaBan(List<int> maHDList);

    }
}
