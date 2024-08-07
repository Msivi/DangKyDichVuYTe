﻿using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend_DV_YTe.Repository
{
    public class CTNhapThietBiYTeRepository:ICTNhapThietBiYTeRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public CTNhapThietBiYTeRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        //public async Task<string> CreateCTNhapThietBiYTe(CTNhapThietBiYTeEntity ctNhap)
        //{
        //    var existingCTNhapThietBiYTe = await _context.cTNhapThietBiYTeEntities
        //        .FirstOrDefaultAsync(c => c.MaThietBiYTe == ctNhap.MaThietBiYTe && c.MaNhapThietBiYTe == ctNhap.MaNhapThietBiYTe && (c.DeletedTime == null || c.DeletedTime != null));

        //    //var phieuNhap = await _context.nhapThietBiYTeEntities.FirstOrDefaultAsync( c => c.Id==ctNhap.MaNhapThietBiYTe && c.DeletedTime == null);
        //    //if (existingCTNhapThietBiYTe != null)
        //    //{
        //    //    existingCTNhapThietBiYTe.soLuong += ctNhap.soLuong;
        //    //    _context.cTNhapThietBiYTeEntities.Update(existingCTNhapThietBiYTe);

        //    //    var existingThietBiYTe = await _context.thietBiYTeEntities
        //    //        .FirstOrDefaultAsync(c => c.Id == ctNhap.MaThietBiYTe && c.DeletedTime == null);
        //    //    if (existingThietBiYTe != null)
        //    //    {
        //    //        existingThietBiYTe.soLuong += ctNhap.soLuong;
        //    //        _context.thietBiYTeEntities.Update(existingThietBiYTe);
        //    //    }


        //    //    phieuNhap.tongTien += ctNhap.soLuong * ctNhap.donGiaNhap;
        //    //    _context.nhapThietBiYTeEntities.Update(phieuNhap);

        //    //}
        //    //else
        //    //{
        //    //    var existingMaThietBiYTe = await _context.thietBiYTeEntities
        //    //        .FirstOrDefaultAsync(c => c.Id == ctNhap.MaThietBiYTe && c.DeletedTime == null);
        //    //    if (existingMaThietBiYTe == null)
        //    //    {
        //    //        throw new Exception(message: "Mã thiết bị không tồn tại!");
        //    //    }


        //    //    if (phieuNhap == null)
        //    //    {
        //    //        throw new Exception(message: "Mã nhập thiết bị không tồn tại!");
        //    //    }

        //    //    existingMaThietBiYTe.soLuong = (existingMaThietBiYTe.soLuong != null ? existingMaThietBiYTe.soLuong : 0) + ctNhap.soLuong;
        //    //    _context.thietBiYTeEntities.Update(existingMaThietBiYTe);

        //    //    var mapctNhap = _mapper.Map<CTNhapThietBiYTeEntity>(ctNhap);
        //    //    await _context.cTNhapThietBiYTeEntities.AddAsync(mapctNhap);

        //    //    phieuNhap.tongTien += ctNhap.soLuong * ctNhap.donGiaNhap;
        //    //    _context.nhapThietBiYTeEntities.Update(phieuNhap);
        //    //}

        //    //await _context.SaveChangesAsync();

        //    return $"{ctNhap.MaThietBiYTe}-{ctNhap.MaNhapThietBiYTe}";
        //}
        public async Task<string> AddNhapThietBiAsync(NhapThietBiDto nhapThietBiDto)
        {
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            if (userIdBytes == null || userIdBytes.Length != sizeof(int))
            {
                throw new Exception("Vui lòng đăng nhập!");
            }

            int userId = BitConverter.ToInt32(userIdBytes, 0);
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var nhapThietBi = new NhapThietBiYTeEntity
                    {
                        ngayTao = DateTime.Now,
                        MaNhanVien = userId,
                        MaNhaCungCap = nhapThietBiDto.MaNhaCungCap,
                        //tongTien = nhapThuocDto.tongTien,
                        CTNhapThietBiYTe = new List<CTNhapThietBiYTeEntity>()
                    };
                    double tongTien = 0;
                    var tenNCC = await _context.nhaCungCapEntities
                    .Where(c => c.Id == nhapThietBi.MaNhaCungCap && c.DeletedTime == null)
                    .Select(c => c.tenNhaCungCap)
                    .FirstOrDefaultAsync();
                    foreach (var item in nhapThietBiDto.ChiTietNhapThietBi)
                    {
                        var loThietBi = new LoThietBiYTeEntity
                        {
                            ngaySanXuat = item.ngaySanXuat,
                            ngayHetHan = item.ngayHetHan,
                            soLuong = item.soLuong,
                            nhaCungCap = tenNCC,
                            donGiaBan = item.donGiaBan,
                            MaThietbiYTe = item.MaThietBi,
                            CreateBy = userId
                        };

                        _context.loThietBiYTeEntities.Add(loThietBi);
                        await _context.SaveChangesAsync();

                        var chiTietNhap = new CTNhapThietBiYTeEntity
                        {
                            MaThietBiYTe = item.MaThietBi,
                            MaNhapThietBiYTe = nhapThietBi.Id,
                            MaLoThietBi = loThietBi.Id,
                            soLuong = item.soLuong,
                            donGiaNhap = item.donGiaNhap,
                            CreateBy = userId
                        };

                        nhapThietBi.CTNhapThietBiYTe.Add(chiTietNhap);
                        // Tính toán giá trị tongTien
                        tongTien += item.soLuong * item.donGiaNhap;

                        var thietBi = _context.thietBiYTeEntities.FirstOrDefault(c => c.Id == item.MaThietBi && c.DeletedTime == null);
                        thietBi.donGia = item.donGiaBan;
                        _context.thietBiYTeEntities.Update(thietBi);
                    }


                    nhapThietBi.tongTien = tongTien; // Gán giá trị tongTien vào trường tongTien của đối tượng nhapThuoc
                    _context.nhapThietBiYTeEntities.Add(nhapThietBi);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return nhapThietBi.Id.ToString();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<CTNhapThietBiYTeEntity> DeleteCTNhapThietBiYTe(int maThietBiYTe, int maNhapThietBiYTe, bool isPhysical)
        {
            try
            {
                var entity = await _context.cTNhapThietBiYTeEntities.FirstOrDefaultAsync(c => c.MaThietBiYTe == maThietBiYTe && c.MaNhapThietBiYTe == maNhapThietBiYTe && c.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy Id chi tiết nhập!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.cTNhapThietBiYTeEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.cTNhapThietBiYTeEntities.Update(entity);
                    }

                    await _context.SaveChangesAsync();

                }
                return entity;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<CTNhapThietBiYTeEntity>> GetAllCTNhapThietBiYTe()
        {
            try
            {
                var entities = await _context.cTNhapThietBiYTeEntities
                     .Where(c => c.DeletedTime == null)
                     .ToListAsync();

                if (entities is null)
                {
                    throw new Exception("Empty list!");
                }
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CTNhapThietBiYTeEntity> GetCTNhapThietBiYTeById(int maThietBiYTe, int maNhapThietBiYTe)
        {
            var entity = await _context.cTNhapThietBiYTeEntities.FirstOrDefaultAsync(c => c.MaThietBiYTe == maThietBiYTe && c.MaNhapThietBiYTe == maNhapThietBiYTe && c.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<CTNhapThietBiYTeEntity>> SearchCTNhapThietBiYTe(string searchKey)
        //{
        //    if (searchKey is null)
        //    {
        //        throw new Exception("Search key rỗng");
        //    }

        //    var ListKH = await _context.cTNhapThietBiYTeEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SoLuong.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}
        public Dictionary<int, int> GenerateNhapThuocReport(DateTime startTime, DateTime endTime)
        {

            var CTNhapThietBiYTe = _context.cTNhapThietBiYTeEntities
                .Where(c => c.CreateTimes >= startTime && c.CreateTimes <= endTime)
                .ToList();

            // Tạo một từ điển để lưu trữ số lượng vaccine đã xuất của từng MaThuoc
            var vaccineReport = new Dictionary<int, int>();

            // Lặp qua danh sách CTNhapVaccineEntity
            foreach (var ctNhap in CTNhapThietBiYTe)
            {
                int maVaccine = ctNhap.MaThietBiYTe;

                // Nếu MaThuoc đã tồn tại trong báo cáo, cộng dồn số lượng đã xuất
                if (vaccineReport.ContainsKey(maVaccine))
                {
                    vaccineReport[maVaccine] += ctNhap.soLuong;
                }
                // Nếu MaThuoc chưa tồn tại trong báo cáo, thêm mới và gán số lượng đã xuất
                else
                {
                    vaccineReport[maVaccine] = ctNhap.soLuong;
                }
            }

            return vaccineReport;
        }
        public async Task UpdateCTNhapThietBiYTe(int maThietBiYTe, int maNhapThietBiYTe, CTNhapThietBiYTeEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "Thông tin cập nhật rỗng!");
            }

            //var existingCTNhapThietBiYTe = await _context.cTNhapThietBiYTeEntities
            //    .FirstOrDefaultAsync(c => c.MaThietBiYTe == maThietBiYTe && c.MaNhapThietBiYTe == maNhapThietBiYTe && c.DeletedTime == null);

            //if (existingCTNhapThietBiYTe == null)
            //{
            //    throw new Exception(message: "Không tìm thấy chi tiết nhập Thuoc!");
            //}

            //var existingMaThuoc = await _context.thietBiYTeEntities
            //   .FirstOrDefaultAsync(c => c.Id == entity.MaThietBiYTe && c.DeletedTime == null);
            //if (existingMaThuoc == null)
            //{
            //    throw new Exception(message: "Mã thiết bị không tồn tại!");
            //}

            //var existingMaNhap = await _context.nhapThietBiYTeEntities
            //    .FirstOrDefaultAsync(c => c.Id == entity.MaNhapThietBiYTe && c.DeletedTime == null);
            //if (existingMaNhap == null)
            //{
            //    throw new Exception(message: "Mã nhập thiết bị không tồn tại!");
            //}

            //if (existingCTNhapThietBiYTe.soLuong > entity.soLuong)
            //{
            //    existingMaThuoc.soLuong = (existingMaThuoc.soLuong != null ? existingMaThuoc.soLuong : 0)

            //        - (existingCTNhapThietBiYTe.soLuong - entity.soLuong);
            //    if (existingMaThuoc.soLuong < 0)
            //    {
            //        throw new Exception(message: "Số lượng trong kho không đủ để thực hiện cập nhật!");
            //    }
            //}
            //else
            //{
            //    if (existingMaThuoc.soLuong < (entity.soLuong - existingCTNhapThietBiYTe.soLuong))
            //    {
            //        throw new Exception(message: $"Số lượng hiện tại trong kho không đủ để thực hiện cập nhật này! Số lượng hiện tại kho đang là: {existingMaThuoc.soLuong}");
            //    }
            //    else
            //    {
            //        existingMaThuoc.soLuong = (existingMaThuoc.soLuong != null ? existingMaThuoc.soLuong : 0)

            //            + (entity.soLuong - existingCTNhapThietBiYTe.soLuong);
            //    }

            //}

            //_context.Entry(existingCTNhapThietBiYTe).CurrentValues.SetValues(entity);
            //await _context.SaveChangesAsync();
        }
    }
}
