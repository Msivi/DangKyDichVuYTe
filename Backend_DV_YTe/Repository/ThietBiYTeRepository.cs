using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Backend_DV_YTe.Model.SanPhamHoaDon;

namespace Backend_DV_YTe.Repository
{
    public class ThietBiYTeRepository:IThietBiYteRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHoaDonRepository _hoaDonrepository;
        private readonly IDiaChiRepository _diaChiRepository;
        public ThietBiYTeRepository(AppDbContext Context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IHoaDonRepository hoaDonRepository)
        {
            _context = Context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _hoaDonrepository = hoaDonRepository;
        }
        public async Task<string> CreateThietBiYTe(ThietBiYTeEntity entity, IFormFile? imageFile)
        {
            var existingThietBiYTe = await _context.thietBiYTeEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            //if (existingThietBiYTe != null)
            //{
            //    throw new Exception(message: "Id is already exist!");
            //}
 
            var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            entity.hinhAnh = $"/Images/" + uniqueFileName;

            var mapEntity = _mapper.Map<ThietBiYTeEntity>(entity);
            mapEntity.donGia = 0;

            await _context.thietBiYTeEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        public async Task<ThietBiYTeEntity> DeleteThietBiYTe(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.thietBiYTeEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.thietBiYTeEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.thietBiYTeEntities.Update(entity);
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

        public async Task<ICollection<AllThietBiModel>> GetAllThietBiYTe()
        {
            try
            {
                var entities = await _context.thietBiYTeEntities
                     .Where(c => c.DeletedTime == null)

                     .Select(t => new AllThietBiModel
                     {
                         id = t.Id,
                         tenThietBiYTe = t.tenThietBi,
                         donGia = t.donGia,
                         soLuong = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                         ngaySanXuat = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                         ngayHetHan = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                         nhaSanXuat = t.nhaSanXuat,
                         tenDanhMuc = t.LoaiThietBi.tenLoaiThietBi,
                         moTa = t.moTa,
                         hinhAnh = t.hinhAnh
                     })
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
        public async Task<ICollection<AllThietBiModel>> GetAllThietBiByMaLoaiTB(int maL)
        {
            try
            {
                var entities = await _context.thietBiYTeEntities
                     .Where(c =>c.MaLoaiThietBi==maL && c.DeletedTime == null)

                     .Include(t => t.LoaiThietBi)
                    .Where(c => c.MaLoaiThietBi == maL && c.DeletedTime == null)
                    .Select(t => new AllThietBiModel
                    {
                        id = t.Id,
                        tenThietBiYTe = t.tenThietBi,
                        donGia = t.donGia,
                        soLuong = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                        ngaySanXuat = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                        ngayHetHan = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                        nhaSanXuat = t.nhaSanXuat,
                        tenDanhMuc = t.LoaiThietBi.tenLoaiThietBi,
                        moTa = t.moTa,
                        hinhAnh = t.hinhAnh
                    })
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
        public async Task<AllThietBiModel> GetThietBiYTeById(int id)
        {
            var entity = await _context.thietBiYTeEntities
                    .Where(c => c.Id == id && c.DeletedTime == null)
                    .Select(t => new AllThietBiModel
                    {
                        id = t.Id,
                        tenThietBiYTe = t.tenThietBi,
                        donGia = t.donGia,
                        soLuong = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                        ngaySanXuat = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                        ngayHetHan = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                        nhaSanXuat = t.nhaSanXuat,
                        tenDanhMuc = t.LoaiThietBi.tenLoaiThietBi,
                        moTa = t.moTa,
                        hinhAnh = t.hinhAnh
                    })
                    .FirstOrDefaultAsync();
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task<ICollection<AllThietBiModel>> SearchThietBiYTe(string searchKey)
        {
            // Lấy danh sách các thuốc từ database
            var query = _context.thietBiYTeEntities
                .Include(t => t.LoaiThietBi)
                .Where(c => c.DeletedTime == null);

            // Chuyển đổi dữ liệu sang bộ nhớ
            var entities = await query.ToListAsync();

            // Áp dụng tìm kiếm bằng cách sử dụng LINQ to Objects
            var filteredEntities = entities
                .Where(c => c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                            c.tenThietBi.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                            (c.nhaSanXuat != null && c.nhaSanXuat.Contains(searchKey, StringComparison.OrdinalIgnoreCase)))
                 .Select(t => new AllThietBiModel
                 {
                     id = t.Id,
                     tenThietBiYTe = t.tenThietBi,
                     donGia = t.donGia,
                     soLuong = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                     ngaySanXuat = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                     ngayHetHan = _context.loThietBiYTeEntities
                            .Where(l => l.MaThietbiYTe == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                     nhaSanXuat = t.nhaSanXuat,
                     tenDanhMuc = t.LoaiThietBi.tenLoaiThietBi,
                     moTa = t.moTa,
                     hinhAnh = t.hinhAnh
                 })
                  .ToList();

            return filteredEntities;
        }

        public async Task UpdateThietBiYTe(int id, ThietBiYTeEntity entity, IFormFile? imageFile)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingThietBiYTe = await _context.thietBiYTeEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingThietBiYTe == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }


            existingThietBiYTe.tenThietBi = entity.tenThietBi;
            existingThietBiYTe.donGia = entity.donGia;
            //existingThietBiYTe.soLuong = entity.soLuong;
            //existingThietBiYTe.ngaySanXuat = entity.ngaySanXuat;
            //existingThietBiYTe.ngayHetHan = entity.ngayHetHan;
            existingThietBiYTe.nhaSanXuat = entity.nhaSanXuat;
            existingThietBiYTe.moTa = entity.moTa;
            existingThietBiYTe.hinhAnh= $"/Images/" + uniqueFileName;

            _context.thietBiYTeEntities.Update(existingThietBiYTe);
            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> DownloadPdfFile(int entity)
        {
            try
            {
                var danhSachXuatThuoc = _context.thietBiYTeEntities
                     .Include(t=>t.LoaiThietBi)
                    .Where(c => c.MaLoaiThietBi == entity && c.DeletedTime == null)
                    .Select(t => new ThietBiYTeEntity
                    {
                        Id = t.Id,
                        tenThietBi = t.tenThietBi,
                        donGia = t.donGia,
                        //soLuong = t.soLuong,
                        //ngaySanXuat = t.ngaySanXuat,
                        //ngayHetHan = t.ngayHetHan,
                        nhaSanXuat = t.nhaSanXuat,
                                            
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
                        PdfPTable table = new PdfPTable(7); // 10 cột trong bảng
                        float[] columnWidths = new float[] { 1f, 5f, 2f, 1.5f, 1.5f, 1.5f, 2f };
                        table.SetWidths(columnWidths);

                        string fontRelativePath = Path.Combine("windsorb.ttf");
                        string fontAbsolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fontRelativePath);
                        // Thiết lập font cho tiêu đề cột
                        BaseFont baseFont = BaseFont.CreateFont(fontAbsolutePath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        Font columnHeaderFont = new Font(baseFont, 12, Font.BOLD);
                        Font columnHeaderFontnd = new Font(baseFont, 8);
                        // Thêm tiêu đề cột vào bảng
                        table.AddCell(new PdfPCell(new Phrase("Mã", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Tên thiet bi", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("giá", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("SL", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("NSX", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("NHH", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Nhà sản xuất", columnHeaderFont)));
                         


                        // Sử dụng danh sách để tạo PDF
                        foreach (var entity1 in danhSachXuatThuoc)
                        {
                            table.AddCell(new PdfPCell(new Phrase(entity1.Id.ToString(), columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.tenThietBi, columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.donGia.ToString(), columnHeaderFontnd)));
                            //table.AddCell(new PdfPCell(new Phrase(entity1.soLuong.ToString(), columnHeaderFontnd)));
                            //table.AddCell(new PdfPCell(new Phrase(entity1.ngaySanXuat.ToString(), columnHeaderFontnd)));
                            //table.AddCell(new PdfPCell(new Phrase(entity1.ngayHetHan.ToString(), columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.nhaSanXuat, columnHeaderFontnd)));
                             
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

        //public void PrintPdf(string pdfPath)
        //{
        //    string fontRelativePath = System.IO.Path.Combine("gswin64c.exe");
        //    string fontAbsolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fontRelativePath); // Đường dẫn đến GhostScript
        //    var args = $"-dPrinted -dBATCH -dNOPAUSE -sDEVICE=mswinpr2 -sOutputFile=%printer%\"Printer Name\" \"{pdfPath}\"";

        //    var process = new System.Diagnostics.Process();
        //    process.StartInfo.FileName = fontAbsolutePath;
        //    process.StartInfo.Arguments = args;
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardError = true;
        //    process.Start();

        //    process.WaitForExit();
        //}

        //public async Task<byte[]> CreatePdfInvoice(int maHD)
        //{
        //    var invoice = await _hoaDonrepository.GetHoaDonById(maHD);
        //    var sanPhamList = await _hoaDonrepository.GetAllSanPhamByMaHD(maHD);

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        var document = new Document();
        //        var writer = PdfWriter.GetInstance(document, memoryStream);
        //        document.Open();

        //        var titleFont = FontFactory.GetFont("Arial", 20, Font.BOLD);
        //        var contentFont = FontFactory.GetFont("Arial", 12);

        //        // Thêm tiêu đề
        //        var title = new Paragraph("Phiếu Thanh Toán", titleFont)
        //        {
        //            Alignment = Element.ALIGN_CENTER
        //        };
        //        document.Add(title);

        //        // Thêm thông tin hóa đơn
        //        document.Add(new Paragraph($"Ngày Mua: {invoice.ngayMua}", contentFont));
        //        document.Add(new Paragraph($"Tổng Tiền: {invoice.tongTien:C}", contentFont));
        //        document.Add(new Paragraph($"Trạng Thái: {invoice.trangThai}", contentFont));
        //        document.Add(new Paragraph($"Địa Chỉ: {invoice.diaChi}", contentFont));
        //        document.Add(new Paragraph($"Ghi Chú: {invoice.ghiChu ?? "Không"}", contentFont));

        //        // Thêm bảng chứa thông tin sản phẩm
        //        var table = new PdfPTable(5) { WidthPercentage = 100 };
        //        table.AddCell(new Phrase("Mã Sản Phẩm", contentFont));
        //        table.AddCell(new Phrase("Tên Sản Phẩm", contentFont));
        //        table.AddCell(new Phrase("Số Lượng", contentFont));
        //        table.AddCell(new Phrase("Đơn Giá", contentFont));
        //        table.AddCell(new Phrase("Thành Tiền", contentFont));

        //        foreach (var sp in sanPhamList)
        //        {
        //            table.AddCell(new Phrase(sp.maSanPham.ToString(), contentFont));
        //            table.AddCell(new Phrase(sp.tenSanPham, contentFont));
        //            table.AddCell(new Phrase(sp.soLuong.ToString(), contentFont));
        //            table.AddCell(new Phrase(sp.donGia.ToString("C"), contentFont));
        //            table.AddCell(new Phrase(sp.thanhTien.ToString("C"), contentFont));
        //        }

        //        document.Add(table);
        //        document.Close();
        //        writer.Close();

        //        return memoryStream.ToArray();
        //    }
        //}
        public async Task<byte[]> CreatePdfInvoices(List<int> maHDList)
        {
            if (maHDList == null || maHDList.Count == 0)
            {
                throw new ArgumentException("The list of invoice IDs cannot be null or empty.", nameof(maHDList));
            }

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document();
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Đường dẫn đến font chữ hỗ trợ tiếng Việt
                string fontRelativePath = Path.Combine("arial.ttf");
                string fontAbsolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fontRelativePath);

                var baseFont = BaseFont.CreateFont(fontAbsolutePath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                var titleFont = new Font(baseFont, 20, Font.BOLD);
                var headerFont = new Font(baseFont, 12, Font.BOLD);
                var contentFont = new Font(baseFont, 12);

                // Thêm thông tin cửa hàng
                var clinicName = new Paragraph("Dịch Vụ Chăm Sóc Y Tế Trực Tuyến", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(clinicName);

                var clinicAddress = new Paragraph("60 Lê Trọng Tấn, Tân Phú, TP HCM", contentFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(clinicAddress);

                // Thêm tiêu đề
                var title = new Paragraph("Phiếu Thanh Toán", headerFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);

                double tongTienTatCaHoaDon = 0;

                // Thêm thông tin hóa đơn chung (chỉ cần hiển thị một lần)
                var invoice = await _hoaDonrepository.GetHoaDonById(maHDList.First());

                if (invoice == null)
                {
                    throw new InvalidOperationException("Invoice not found.");
                }

                int idDc = (int)invoice.diaChi;
                
                    var diaChi = await _context.diaChiEntities.FirstOrDefaultAsync(c=>c.Id==idDc && c.DeletedTime==null);
                    if (diaChi != null)
                    {
                        document.Add(new Paragraph($"Địa Chỉ: {diaChi.tenDiaChi}", contentFont));
                    }
                

                document.Add(new Paragraph($"Ngày Mua: {invoice.ngayMua}", contentFont));
                document.Add(new Paragraph($"Ghi Chú: {invoice.ghiChu ?? "Không"}", contentFont));

                string maHDListStr = string.Join(", ", maHDList);
                document.Add(new Paragraph($"Mã hóa đơn: {maHDListStr}", contentFont));

                // Thêm bảng chứa thông tin sản phẩm
                var table = new PdfPTable(5) { WidthPercentage = 100 };
                table.AddCell(new Phrase("Mã Sản Phẩm", contentFont));
                table.AddCell(new Phrase("Tên Sản Phẩm", contentFont));
                table.AddCell(new Phrase("Số Lượng", contentFont));
                table.AddCell(new Phrase("Đơn Giá", contentFont));
                table.AddCell(new Phrase("Thành Tiền", contentFont));

                foreach (var maHD in maHDList)
                {
                    invoice = await _hoaDonrepository.GetHoaDonById(maHD);
                    var sanPhamList = await _hoaDonrepository.GetAllSanPhamByMaHD(maHD);

                    // Cộng tổng tiền của hóa đơn hiện tại vào tổng tiền của tất cả hóa đơn
                    tongTienTatCaHoaDon += invoice.tongTien;

                    foreach (var sp in sanPhamList)
                    {
                        table.AddCell(new Phrase(sp.maSanPham.ToString(), contentFont));
                        table.AddCell(new Phrase(sp.tenSanPham, contentFont));
                        table.AddCell(new Phrase(sp.soLuong.ToString(), contentFont));
                        table.AddCell(new Phrase(sp.donGia.ToString("C"), contentFont));
                        table.AddCell(new Phrase(sp.thanhTien.ToString("C"), contentFont));
                    }

                    // Thêm khoảng trống giữa các hóa đơn (tùy chọn)
                    document.Add(new Paragraph("\n", contentFont));
                }

                document.Add(table);

                // Thêm tổng tiền của tất cả hóa đơn
                //document.Add(new Paragraph($"Tổng Tiền Tất Cả Hóa Đơn: {tongTienTatCaHoaDon:C}", headerFont));

                var tongTien = new Paragraph($"Tổng Tiền Tất Cả Hóa Đơn: {tongTienTatCaHoaDon:C}", headerFont)
                {
                    Alignment = Element.ALIGN_RIGHT
                };
               document.Add(tongTien);
                //// Lời cảm ơn
                //var footer = new Paragraph("Cảm ơn quý khách, hẹn gặp lại! ", contentFont)
                //{
                //    Alignment = Element.ALIGN_CENTER
                //};
                //document.Add(footer);

                document.Close();
                writer.Close();

                return memoryStream.ToArray();
            }
        }
        public async Task<string> CapNhatSoLuongDaBan(List<int> maHDList)
        {
            foreach (var maHoaDon in maHDList)
            {
                // Lấy danh sách sản phẩm từ hóa đơn
                var sanPhamList = await _hoaDonrepository.GetAllSanPhamByMaHD(maHoaDon);

                foreach (var sanPham in sanPhamList)
                {
                    if (sanPham is thuocMuaModel thuoc)
                    {
                        // Lấy danh sách lô thuốc từ kho
                        var loThuocList = await _context.loThuocEntities
                                              .Where(t => t.MaThuoc == thuoc.maSanPham && t.soLuong > 0 && t.DeletedTime==null)
                                              .OrderBy(t => t.CreateTimes)
                                              .ToListAsync();
                        if (!loThuocList.Any())
                        {
                            throw new Exception($"Không tìm thấy thuốc có mã {thuoc.maSanPham} trong kho.");
                        }
                        // Trừ số lượng đã bán từ số lượng tồn kho
                        int soLuongCanTru = thuoc.soLuong;
                        foreach (var loThuoc in loThuocList)
                        {
                            if (soLuongCanTru <= 0)
                            {
                                break;
                            }

                            if (loThuoc.soLuong >= soLuongCanTru)
                            {
                                loThuoc.soLuong -= soLuongCanTru;
                                soLuongCanTru = 0;
                            }
                            else
                            {
                                soLuongCanTru -= loThuoc.soLuong;
                                loThuoc.soLuong = 0;
                            }

                            _context.loThuocEntities.Update(loThuoc);
                        }

                        if (soLuongCanTru > 0)
                        {
                            throw new Exception($"Không đủ số lượng thuốc có mã {thuoc.maSanPham} trong kho.");
                        }
                    }
                    else if (sanPham is thiBiMuaModel thietBi)
                    {
                        // Lấy danh sách lô thiết bị y tế từ kho
                        var loThietBiList = await _context.loThietBiYTeEntities
                                              .Where(tb => tb.MaThietbiYTe == thietBi.maSanPham && tb.soLuong > 0)
                                              .OrderBy(tb => tb.CreateTimes)
                                              .ToListAsync();
                        if (!loThietBiList.Any())
                        {
                            throw new Exception($"Không tìm thấy thiết bị y tế có mã {thietBi.maSanPham} trong kho.");
                        }
                        // Trừ số lượng đã bán từ số lượng tồn kho
                        int soLuongCanTru = thietBi.soLuong;
                        foreach (var loThietBi in loThietBiList)
                        {
                            if (soLuongCanTru <= 0)
                            {
                                break;
                            }

                            if (loThietBi.soLuong >= soLuongCanTru)
                            {
                                loThietBi.soLuong -= soLuongCanTru;
                                soLuongCanTru = 0;
                            }
                            else
                            {
                                soLuongCanTru -= loThietBi.soLuong;
                                loThietBi.soLuong = 0;
                            }

                            _context.loThietBiYTeEntities.Update(loThietBi);
                        }

                        if (soLuongCanTru > 0)
                        {
                            throw new Exception($"Không đủ số lượng thiết bị y tế có mã {thietBi.maSanPham} trong kho.");
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            return "Cập nhật số lượng đã bán thành công.";
        }




        public async Task PrintPdfs(List<byte[]> pdfBytesList)
        {
            foreach (var pdfBytes in pdfBytesList)
            {
                var pdfPath = Path.Combine(Path.GetTempPath(), $"invoice_{Guid.NewGuid()}.pdf");
                await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);
                PrintPdf(pdfPath);
            }
        }
        public void PrintPdf(string pdfPath)
        {
            string fontRelativePath = System.IO.Path.Combine("gswin64c.exe");
            string fontAbsolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fontRelativePath); // Đường dẫn đến GhostScript
            var args = $"-dPrinted -dBATCH -dNOPAUSE -sDEVICE=mswinpr2 -sOutputFile=%printer%\"Printer Name\" \"{pdfPath}\"";

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = fontAbsolutePath;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            process.WaitForExit();
        }
    }
}
