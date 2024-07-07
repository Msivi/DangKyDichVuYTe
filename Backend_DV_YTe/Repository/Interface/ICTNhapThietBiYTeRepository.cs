﻿using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ICTNhapThietBiYTeRepository
    {
        Task<ICollection<CTNhapThietBiYTeEntity>> GetAllCTNhapThietBiYTe();
        Task<CTNhapThietBiYTeEntity> GetCTNhapThietBiYTeById(int maThuoc, int maNhapThuoc);
        //Task<ICollection<CTNhapThietBiYTeEntity>> SearchCTNhapThietBiYTe(string searchKey);
        //Task<string> CreateCTNhapThietBiYTe(CTNhapThietBiYTeEntity entity);
        Task<string> AddNhapThietBiAsync(NhapThietBiDto nhapThietBiDto);
        Task UpdateCTNhapThietBiYTe(int maThuoc, int maNhapThuoc, CTNhapThietBiYTeEntity entity);
        Task<CTNhapThietBiYTeEntity> DeleteCTNhapThietBiYTe(int maThietBi, int maNhapThietBi, bool isPhysical);
        Dictionary<int, int> GenerateNhapThuocReport(DateTime startTime, DateTime endTime);
    }
}
