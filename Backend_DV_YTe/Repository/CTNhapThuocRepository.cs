using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class CTNhapThuocRepository:ICTNhapThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CTNhapThuocRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateCTNhapThuoc(CTNhapThuocEntity ctNhap)
        {
            var existingCTNhapThuoc = await _context.cTNhapThuocEntities
                .FirstOrDefaultAsync(c => c.MaThuoc == ctNhap.MaThuoc && c.MaNhapThuoc == ctNhap.MaNhapThuoc && (c.DeletedTime == null || c.DeletedTime != null));
            if (existingCTNhapThuoc != null)
            {
                existingCTNhapThuoc.soLuong += ctNhap.soLuong;
                _context.cTNhapThuocEntities.Update(existingCTNhapThuoc);

                var existingMaThuoc = await _context.thuocEntities
                    .FirstOrDefaultAsync(c => c.Id == ctNhap.MaThuoc && c.DeletedTime == null);
                if (existingMaThuoc != null)
                {
                    existingMaThuoc.soLuong += ctNhap.soLuong;
                    _context.thuocEntities.Update(existingMaThuoc);
                }
            }
            else
            {
                var existingMaThuoc = await _context.thuocEntities
                    .FirstOrDefaultAsync(c => c.Id == ctNhap.MaThuoc && c.DeletedTime == null);
                if (existingMaThuoc == null)
                {
                    throw new Exception(message: "Mã thuoc không tồn tại!");
                }

                var existingMaNhap = await _context.nhapThuocEntities
                    .FirstOrDefaultAsync(c => c.Id == ctNhap.MaNhapThuoc && c.DeletedTime == null);
                if (existingMaNhap == null)
                {
                    throw new Exception(message: "Mã nhập thuoc không tồn tại!");
                }

                existingMaThuoc.soLuong = (existingMaThuoc.soLuong != null ? existingMaThuoc.soLuong : 0) + ctNhap.soLuong;
                _context.thuocEntities.Update(existingMaThuoc);

                var mapctNhap = _mapper.Map<CTNhapThuocEntity>(ctNhap);
                await _context.cTNhapThuocEntities.AddAsync(mapctNhap);
            }

            await _context.SaveChangesAsync();

            return $"{ctNhap.MaThuoc}-{ctNhap.MaNhapThuoc}";
        }

        public async Task<CTNhapThuocEntity> DeleteCTNhapThuoc(int maThuoc, int maNhapThuoc, bool isPhysical)
        {
            try
            {
                var entity = await _context.cTNhapThuocEntities.FirstOrDefaultAsync(c => c.MaThuoc == maThuoc && c.MaNhapThuoc == maNhapThuoc && c.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy Id chi tiết nhập vaccine!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.cTNhapThuocEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.cTNhapThuocEntities.Update(entity);
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

        public async Task<ICollection<CTNhapThuocEntity>> GetAllCTNhapThuoc()
        {
            try
            {
                var entities = await _context.cTNhapThuocEntities
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

        public async Task<CTNhapThuocEntity> GetCTNhapThuocById(int maThuoc, int maNhapThuoc)
        {
            var entity = await _context.cTNhapThuocEntities.FirstOrDefaultAsync(c => c.MaThuoc == maThuoc && c.MaNhapThuoc == maNhapThuoc && c.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        //public async Task<ICollection<CTNhapThuocEntity>> SearchCTNhapThuoc(string searchKey)
        //{
        //    if (searchKey is null)
        //    {
        //        throw new Exception("Search key rỗng");
        //    }

        //    var ListKH = await _context.cTNhapThuocEntities.ToListAsync();

        //    // Filter the list and materialize the results
        //    var filteredList = ListKH.Where(c => (
        //        c.Id.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
        //        c.SoLuong.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
        //    ) && c.DeletedTime == null).ToList();

        //    return filteredList;
        //}
        public Dictionary<int, int> GenerateNhapThuocReport(DateTime startTime, DateTime endTime)
        {

            var ctNhapThuoc = _context.cTNhapThuocEntities
                .Where(c => c.CreateTimes >= startTime && c.CreateTimes <= endTime)
                .ToList();

            // Tạo một từ điển để lưu trữ số lượng vaccine đã xuất của từng MaThuoc
            var vaccineReport = new Dictionary<int, int>();

            // Lặp qua danh sách CTNhapVaccineEntity
            foreach (var ctNhap in ctNhapThuoc)
            {
                int maVaccine = ctNhap.MaThuoc;

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
        public async Task UpdateCTNhapThuoc(int maThuoc, int maNhapThuoc, CTNhapThuocEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "Thông tin cập nhật rỗng!");
            }

            var existingCTNhapThuoc = await _context.cTNhapThuocEntities
                .FirstOrDefaultAsync(c => c.MaThuoc== maThuoc && c.MaNhapThuoc == maNhapThuoc && c.DeletedTime == null);

            if (existingCTNhapThuoc == null)
            {
                throw new Exception(message: "Không tìm thấy chi tiết nhập Thuoc!");
            }

            var existingMaThuoc = await _context.thuocEntities
               .FirstOrDefaultAsync(c => c.Id == entity.MaThuoc&& c.DeletedTime == null);
            if (existingMaThuoc == null)
            {
                throw new Exception(message: "Mã Vaccine không tồn tại!");
            }

            var existingMaNhap = await _context.nhapThuocEntities
                .FirstOrDefaultAsync(c => c.Id == entity.MaNhapThuoc && c.DeletedTime == null);
            if (existingMaNhap == null)
            {
                throw new Exception(message: "Mã nhập Thuoc không tồn tại!");
            }

            if (existingCTNhapThuoc.soLuong > entity.soLuong)
            {
                existingMaThuoc.soLuong = (existingMaThuoc.soLuong != null ? existingMaThuoc.soLuong : 0)

                    - (existingCTNhapThuoc.soLuong - entity.soLuong);
                if (existingMaThuoc.soLuong < 0)
                {
                    throw new Exception(message: "Số lượng trong kho không đủ để thực hiện cập nhật!");
                }
            }
            else
            {
                if (existingMaThuoc.soLuong < (entity.soLuong - existingCTNhapThuoc.soLuong))
                {
                    throw new Exception(message: $"Số lượng hiện tại trong kho không đủ để thực hiện cập nhật này! Số lượng hiện tại kho đang là: {existingMaThuoc.soLuong}");
                }
                else
                {
                    existingMaThuoc.soLuong = (existingMaThuoc.soLuong != null ? existingMaThuoc.soLuong : 0)

                        + (entity.soLuong - existingCTNhapThuoc.soLuong);
                }

            }

            _context.Entry(existingCTNhapThuoc).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }
    }
}
