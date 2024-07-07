using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ClosedXML.Excel;

namespace Backend_DV_YTe.Repository
{
    public class DichVuRepository:IDichVuRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DichVuRepository(AppDbContext Context, IMapper mapper, IDistributedCache distributedCache, IWebHostEnvironment webHostEnvironment)
        {
            _context = Context;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> CreateDichVu(DichVuEntity entity, IFormFile imageFile)
        {
            var existingDichVu = await _context.dichVuEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingDichVu != null)
            {
                throw new Exception(message: "Id is already exist!");
            }
            // Kiểm tra loại tệp ảnh
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new Exception(message: "Loại tệp ảnh không được hỗ trợ. Vui lòng chọn tệp ảnh có định dạng JPG, JPEG, PNG, hoặc GIF.");
            }

            var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            entity.hinhAnh = $"/Images/" + uniqueFileName;

            await _context.dichVuEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.Id.ToString();
        }

        public async Task<DichVuEntity> DeleteDichVu(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.dichVuEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy nhà cung cấp!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.dichVuEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.dichVuEntities.Update(entity);
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

        public async Task<ICollection<DichVuEntity>> GetAllDichVu()
        {
            try
            {
                var entities = await _context.dichVuEntities
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

        public async Task<ICollection<DichVuTheoLoaiModel>> GetDichVuByLoaiDichVuAsync(int? loaiDichVuId, int chuyenKhoaId)
        {
            try
            {
                if(chuyenKhoaId == 0)
                {
                    throw new Exception(message: "Hãy chọn chuyên khoa");
                }

                var query = _context.dichVuEntities.AsQueryable();

                if (loaiDichVuId.HasValue)
                {
                    query = query.Where(dv => dv.MaLoaiDichVu == loaiDichVuId.Value);
                }

                 
               query = query.Where(dv => dv.MaChuyenKhoa == chuyenKhoaId);
               

                var dichVuTheoLoaiModels = await query
                    .Select(dv => new DichVuTheoLoaiModel
                    {
                        MaDichVu=dv.Id,
                        TenDichVu = dv.tenDichVu,
                        Gia = dv.gia,
                        MoTa = dv.moTa,
                        TenChuyenKhoa = dv.ChuyenKhoa.tenChuyenKhoa,
                       HinhAnh=dv.hinhAnh,
                         
                    })
                    .ToListAsync();

                return dichVuTheoLoaiModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<DichVuEntity>> GetAllDichVuTheoChuyenKhoa(string ChuyenKhoa)
        {
            try
            {
                var query = from dichVu in _context.dichVuEntities
                            //join ctBacSi in _context.cTBacSiEntities on dichVu.Id equals ctBacSi.MaDichVu
                            //join bacSi in _context.BacSiEntities on ctBacSi.MaBacSi equals bacSi.Id
                            //where ctBacSi.chuyenKhoa == ChuyenKhoa
                            select dichVu;

                var danhSachDichVuTheoChuyenKhoa = await query.ToListAsync();
                if (danhSachDichVuTheoChuyenKhoa is null)
                {
                    throw new Exception("Empty list!");
                }
                return danhSachDichVuTheoChuyenKhoa;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DichVuEntity> GetDichVuById(int id)
        {
            var entity = await _context.dichVuEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task<ICollection<DichVuEntity>> SearchDichVu(string searchKey)
        {
            var ListKH = await _context.dichVuEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
                c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.tenDichVu.Contains(searchKey, StringComparison.OrdinalIgnoreCase)
                //c.nhaSanXuat.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
            ) && c.DeletedTime == null).ToList();

            return filteredList;
        }

        public async Task UpdateDichVu(int id, DichVuModel entity,IFormFile? imageFile)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingDichVu = await _context.dichVuEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingDichVu == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
            int userId = BitConverter.ToInt32(userIdBytes, 0);


            if (imageFile != null)
            {
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                existingDichVu.hinhAnh = $"/Images/" + uniqueFileName;
            }
            //else
            //{
            //    existingDichVu.hinhAnh = null;
            //}



            existingDichVu.tenDichVu=entity.tenDichVu;
            existingDichVu.moTa= entity.moTa;
            existingDichVu.gia= entity.gia;
            existingDichVu.MaLoaiDichVu = entity.MaLoaiDichVu;
            existingDichVu.MaChuyenKhoa=entity.MaChuyenKhoa;
            existingDichVu.CreateBy = userId;
           

            _context.dichVuEntities.Update(existingDichVu);
            await _context.SaveChangesAsync();
        }

        // thống kê tổng giá trị của các dịch vụ đã được đặt trong khoảng
        public async Task<double> thongKeTinhTongGiaTriDichVu(DateTime fromDate, DateTime toDate)
        {
            var thongke = _context.dichVuEntities
                            .Join(
                                _context.lichHenEntities,
                                dichVu => dichVu.Id, // Khóa chính trong bảng DichVu
                                lichHen => lichHen.MaDichVu, // Khóa ngoại trong bảng LichHen liên kết với DichVu
                                (dichVu, lichHen) => new { DichVu = dichVu, LichHen = lichHen }
                            )
                            .Where(x => x.LichHen.thoiGianDuKien >= fromDate && x.LichHen.thoiGianDuKien <= toDate && x.DichVu.DeletedTime==null )
                            .Sum(x => x.DichVu.gia);

          
            return thongke;
        }
        //public async Task<double> TinhGiaTriTrungBinhDichVuTrongKhoangThoiGian(DateTime ngayBD, DateTime ngayKT)
        //{
        //    return await _context.dichVuEntities
        //        .Where(d => d.LichHen.Any(lh => lh.thoiGianDuKien >= ngayBD && lh.thoiGianDuKien <= ngayKT))
        //        .AverageAsync(d => d.gia);
        //}
        public async Task<List<DichVuSuDung>> TimDichVuDaSuDung(DateTime fromDate, DateTime toDate)
        {
            var dichVuDaSuDung = await _context.dichVuEntities
                                .Join(
                                    _context.lichHenEntities,
                                    dichVu => dichVu.Id, // Khóa chính của bảng DichVuEntities
                                    lichHen => lichHen.MaDichVu, // Khóa ngoại trong bảng LichHen liên kết với DichVuEntities
                                    (dichVu, lichHen) => new { DichVu = dichVu, LichHen = lichHen }
                                )
                                .Where(d => d.LichHen.thoiGianDuKien >= fromDate && d.LichHen.thoiGianDuKien <= toDate && d.LichHen.DeletedTime==null)
                                .GroupBy(d => d.DichVu.tenDichVu)
                                .Select(g => new DichVuSuDung { TenDichVu = g.Key, SoLuong =(int) g.Select(x => x.DichVu.gia).Sum() })
                                .ToListAsync();

            return dichVuDaSuDung;
        }
        public async Task<byte[]> ExportThongKeDichVuToExcel(DateTime fromDate, DateTime toDate)
        {
            double tongGiaTri = await thongKeTinhTongGiaTriDichVu(fromDate, toDate);
            var dichVuSuDung = await TimDichVuDaSuDung(fromDate, toDate);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ThongKeDichVu");
                worksheet.Cell(1, 1).Value = "Tên Dịch Vụ";
                worksheet.Cell(1, 2).Value = "Số Lượng";

                int row = 2;
                foreach (var item in dichVuSuDung)
                {
                    worksheet.Cell(row, 1).Value = item.TenDichVu;
                    worksheet.Cell(row, 2).Value = item.SoLuong;
                    row++;
                }

                worksheet.Cell(row, 1).Value = "Tổng Giá Trị";
                worksheet.Cell(row, 2).Value = tongGiaTri;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        public async Task<byte[]> DownloadPdfFile(int entity)
        {
            try
            {
                var danhSachXuatThuoc = _context.dichVuEntities
                     .Include(t => t.LoaiDichVu)
                    .Where(c => c.MaLoaiDichVu == entity && c.DeletedTime == null)
                    .Select(t => new DichVuEntity
                    {
                        Id = t.Id,
                        tenDichVu = t.tenDichVu,
                        gia = t.gia,
                        moTa=t.moTa,
                        LoaiDichVu= new LoaiDichVuEntity { tenLoai=t.LoaiDichVu.tenLoai}

                    })
                    .ToList();

                if (danhSachXuatThuoc is null || danhSachXuatThuoc.Count == 0)
                {
                    throw new Exception("Danh sách thuốc bị rỗng!");
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var document = new Document())
                    {
                        var writer = PdfWriter.GetInstance(document, memoryStream);
                        document.Open();

                        // Tạo bảng
                        PdfPTable table = new PdfPTable(5); // 10 cột trong bảng
                        float[] columnWidths = new float[] { 1f, 5f, 2f, 4f, 2.5f};
                        table.SetWidths(columnWidths);

                        // Thiết lập đường dẫn tuyệt đối đến file font
                        string fontDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");
                        string fontAbsolutePath = Path.Combine(fontDirectory, "arial.ttf");

                        // Kiểm tra sự tồn tại của file font
                        if (!File.Exists(fontAbsolutePath))
                        {
                            throw new FileNotFoundException("File font không tồn tại tại đường dẫn: " + fontAbsolutePath);
                        }
                        // Thiết lập font cho tiêu đề cột
                        BaseFont baseFont = BaseFont.CreateFont(fontAbsolutePath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        Font columnHeaderFont = new Font(baseFont, 12, Font.BOLD);
                        Font columnHeaderFontnd = new Font(baseFont, 10,Font.NORMAL);
                        // Thêm tiêu đề cột vào bảng
                        table.AddCell(new PdfPCell(new Phrase("Mã", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Tên loai DV", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("giá", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Mô tả", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Danh mục", columnHeaderFont)));
                        


                        // Sử dụng danh sách để tạo PDF
                        foreach (var entity1 in danhSachXuatThuoc)
                        {
                            table.AddCell(new PdfPCell(new Phrase(entity1.Id.ToString(), columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.tenDichVu, columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.gia.ToString(), columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.moTa, columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.LoaiDichVu.tenLoai.ToString(), columnHeaderFontnd)));
                            

                        }

                        // Thêm bảng vào tệp tin PDF
                        document.Add(table);
                    }

                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
