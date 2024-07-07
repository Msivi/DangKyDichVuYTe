﻿using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface INhanVienRepository
    {
        Task<ICollection<NhanVienEntity>> GetAllNhanVien();
        Task<NhanVienEntity> GetNhanVienById(int Id);
        Task<NhanVienEntity> GetTTNhanVien();
        Task<ICollection<NhanVienEntity>> SearchNhanVien(string searchKey);

        Task UpdateNhanVien(updateNhanVienModel entity);
        Task UpdateAvatar(string avatarUrl);
        Task<NhanVienEntity> DeleteNhaVien(int id, bool isPhysical);
    }
}
