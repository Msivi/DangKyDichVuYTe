using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface INhapThuocRepository
    {
        Task<ICollection<NhapThuocEntity>> GetAllNhapThuoc();
        Task<NhapThuocEntity> GetNhapThuocById(int Id);
        //Task<ICollection<NhapThuocEntity>> SearchNhapThuoc(string searchKey);
        Task<string> CreateNhapThuoc(NhapThuocEntity entity);
        Task UpdateNhapThuoc(int id, NhapThuocEntity entity);
        Task<NhapThuocEntity> DeleteNhapThuoc(int entity, bool isPhysical);
    }
}
