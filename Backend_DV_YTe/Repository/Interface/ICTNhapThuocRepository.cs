﻿using Backend_DV_YTe.Entity;

namespace Backend_DV_YTe.Repository.Interface
{
    public interface ICTNhapThuocRepository
    {
        Task<ICollection<CTNhapThuocEntity>> GetAllCTNhapThuoc();
        Task<CTNhapThuocEntity> GetCTNhapThuocById(int maThuoc, int maNhapThuoc);
        //Task<ICollection<CTNhapThuocEntity>> SearchCTNhapThuoc(string searchKey);
        Task<string> CreateCTNhapThuoc(CTNhapThuocEntity entity);
        Task UpdateCTNhapThuoc(int maThuoc, int maNhapThuoc, CTNhapThuocEntity entity);
        Task<CTNhapThuocEntity> DeleteCTNhapThuoc(int maThuoc, int maNhapThuoc, bool isPhysical);
        Dictionary<int, int> GenerateNhapThuocReport(DateTime startTime, DateTime endTime);
    }
}
