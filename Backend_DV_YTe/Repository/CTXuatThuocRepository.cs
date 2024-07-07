using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
 

namespace Backend_DV_YTe.Repository
{
    public class CTXuatThuocRepository:ICTXuatThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMemoryCache _cache;
        public CTXuatThuocRepository(AppDbContext Context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IMemoryCache cache)
        {
            _context = Context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _cache = cache;
        }

        static List<CTXuatThuocEntity> danhSachXuatThuoc = new List<CTXuatThuocEntity>();
  
        public async Task<string> CreateCTXuatThuoc(CTXuatThuocEntity entity)
        {
            //var existingCTXuatThuoc = await _context.cTXuatThuocEntities
            //    .FirstOrDefaultAsync(c => c.MaThuoc == entity.MaThuoc && c.MaXuatThuoc == entity.MaXuatThuoc);
            //var existingSLThuoc = await _context.thuocEntities
            //       .FirstOrDefaultAsync(c => c.Id == entity.MaThuoc && c.DeletedTime == null);

            //if (existingSLThuoc.soLuong < entity.soLuong)
            //{
            //    throw new Exception($"Không đủ số lượng thiết bị để xuất! (Số lượng hiện tại trong kho: {existingSLThuoc.soLuong})");
            //}

            //if (existingCTXuatThuoc != null)
            //{
            //    existingCTXuatThuoc.soLuong += entity.soLuong;
            //    _context.cTXuatThuocEntities.Update(existingCTXuatThuoc);

            //    var existingMaThietBi = await _context.thuocEntities
            //        .FirstOrDefaultAsync(c => c.Id == entity.MaThuoc && c.DeletedTime == null);
            //    if (existingMaThietBi != null)
            //    {
            //        existingMaThietBi.soLuong -= entity.soLuong;
            //        _context.thuocEntities.Update(existingMaThietBi);
            //    }
            //}
            //else
            //{
            //    var existingMaThietBi = await _context.thuocEntities
            //        .FirstOrDefaultAsync(c => c.Id == entity.MaThuoc && c.DeletedTime == null);
            //    if (existingMaThietBi == null)
            //    {
            //        throw new Exception(message: "Mã thuoc không tồn tại!");
            //    }

            //    var existingMaXuat = await _context.xuatThuocEntities
            //        .FirstOrDefaultAsync(c => c.Id == entity.MaXuatThuoc && c.DeletedTime == null);
            //    if (existingMaXuat == null)
            //    {
            //        throw new Exception(message: "Mã xuất thiết bị không tồn tại!");
            //    }

            //    //if (existingMaThietBi.soLuong < entity.soLuong)
            //    //{
            //    //    throw new Exception($"Không đủ số lượng thiết bị để xuất! (Số lượng hiện tại trong kho: {existingMaThietBi.soLuong})");
            //    //}


            //    existingMaThietBi.soLuong -= entity.soLuong;
            //    _context.thuocEntities.Update(existingMaThietBi);

            //    // Thêm phiếu xuất vaccine mới
            //    var mapEntity = _mapper.Map<CTXuatThuocEntity>(entity);
            //    await _context.cTXuatThuocEntities.AddAsync(mapEntity);
            //}

            //await _context.SaveChangesAsync();
            //danhSachXuatThuoc.Add(entity);
            //_cache.Set("danhSachXuatThuoc", danhSachXuatThuoc);

            return $"{entity.MaXuatThuoc}-{entity.MaThuoc}";
        }

        public byte[] DownloadPdfFile()
        {
            try
            {
                BaseFont baseFont = BaseFont.CreateFont("D:\\DoAnKySu\\fontVN\\vps-thanh-hoa.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                using (var document = new iTextSharp.text.Document())
                {
                    var memoryStream = new MemoryStream();
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Tạo bảng
                    PdfPTable table = new PdfPTable(3); // 4 cột trong bảng

                    // Thiết lập font cho tiêu đề cột
                    Font columnHeaderFont = new Font(baseFont, 12, Font.BOLD);

                    // Thêm tiêu đề cột vào bảng
                    table.AddCell(new PdfPCell(new Phrase("Mã thuốc", columnHeaderFont)));
                    table.AddCell(new PdfPCell(new Phrase("Mã xuất thuốc", columnHeaderFont)));
                    table.AddCell(new PdfPCell(new Phrase("Số lượng", columnHeaderFont)));
                    //table.AddCell(new PdfPCell(new Phrase("Ngày tạo", columnHeaderFont)));

                    if (danhSachXuatThuoc is null || danhSachXuatThuoc.Count == 0)
                    {
                        throw new Exception("Danh sách xuất thuốc bị rỗng!");
                    }

                    // Sử dụng danh sách từ list để tạo PDF
                    foreach (var entity in danhSachXuatThuoc)
                    {
                        table.AddCell(new PdfPCell(new Phrase(entity.MaThuoc.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(entity.MaXuatThuoc.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(entity.soLuong.ToString())));
                        //table.AddCell(new PdfPCell(new Phrase(entity.ngayTao.ToString())));
                    }

                    // Thêm bảng vào tệp tin PDF
                    document.Add(table);

                    document.Close();

                    // Chuyển đổi MemoryStream thành mảng byte
                    byte[] bytes = memoryStream.ToArray();

                    danhSachXuatThuoc.Clear();

                    return bytes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Dictionary<int, int> GenerateXuatThuocReport(DateTime startTime, DateTime endTime)
        {

            var CTXuatThuocs = _context.cTXuatThuocEntities
                .Where(c => c.CreateTimes >= startTime.Date && c.CreateTimes <= endTime.Date)
                .ToList();

            var vaccineReport = new Dictionary<int, int>();

            foreach (var CTXuatThuoc in CTXuatThuocs)
            {
                int MaThuoc = CTXuatThuoc.MaThuoc;

                if (vaccineReport.ContainsKey(MaThuoc))
                {
                    vaccineReport[MaThuoc] += CTXuatThuoc.soLuong;
                }
                else
                {
                    vaccineReport[MaThuoc] = CTXuatThuoc.soLuong;
                }
            }

            return vaccineReport;
        }

        public async Task<CTXuatThuocEntity> DeleteCTXuatThuoc(int MaThuoc, int MaXuatThuoc, bool isPhysical)
        {
            try
            {
                var entity = await _context.cTXuatThuocEntities.FirstOrDefaultAsync(e => e.MaThuoc == MaThuoc && e.MaXuatThuoc == MaXuatThuoc && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy Id chi tiết xuất vaccine!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.cTXuatThuocEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.cTXuatThuocEntities.Update(entity);
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

        public async Task<ICollection<CTXuatThuocEntity>> GetAllCTXuatThuoc()
        {
            try
            {
                var entities = await _context.cTXuatThuocEntities
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
        
        public async Task<CTXuatThuocEntity> GetCTXuatThuocById(int MaThuoc, int MaXuatThuoc)
        {
            var entity = await _context.cTXuatThuocEntities.FirstOrDefaultAsync(e => e.MaThuoc == MaThuoc && e.MaXuatThuoc == MaXuatThuoc && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task UpdateCTXuatThuoc(int MaThuoc, int MaXuatThuoc, CTXuatThuocEntity entity)
        {

            //if (entity == null)
            //{
            //    throw new Exception(message: "Thông tin cập nhật rỗng!");
            //}

            //var existingCTXuatThuoc = await _context.cTXuatThuocEntities
            //    .FirstOrDefaultAsync(e => e.MaThuoc == MaThuoc && e.MaXuatThuoc == MaXuatThuoc && e.DeletedTime == null);
            //if (existingCTXuatThuoc == null)
            //{
            //    throw new Exception(message: "Không tìm thấy chi tiết xuất thuoc!");
            //}

            //var existingMaThuoc = await _context.thuocEntities
            //    .FirstOrDefaultAsync(c => c.Id == MaThuoc && c.DeletedTime == null);
              
            //if (existingMaThuoc == null)
            //{
            //    throw new Exception(message: "Mã thuoc không tồn tại!");
            //}

            //var existingMaXuat = await _context.xuatThuocEntities
            //    .FirstOrDefaultAsync(c => c.Id == MaXuatThuoc && c.DeletedTime == null);
            //if (existingMaXuat == null)
            //{
            //    throw new Exception(message: "Mã xuất thuoc không tồn tại!");
            //}

            //if (existingCTXuatThuoc.soLuong > entity.soLuong)
            //{
            //    existingMaThuoc.soLuong = (existingMaThuoc.soLuong != null ? existingMaThuoc.soLuong : 0)

            //        + (existingCTXuatThuoc.soLuong - entity.soLuong);

            //}
            //else
            //{
            //    if (existingMaThuoc.soLuong < (entity.soLuong - existingCTXuatThuoc.soLuong))
            //    {
            //        throw new Exception(message: $"Số lượng hiện tại trong kho không đủ để thực hiện cập nhật này! Số lượng hiện tại kho đang là: {existingMaThuoc.soLuong}");
            //    }
            //    else
            //    {
            //        existingMaThuoc.soLuong = (existingMaThuoc.soLuong != null ? existingMaThuoc.soLuong : 0)

            //            - (entity.soLuong - existingCTXuatThuoc.soLuong);
            //    }

            //}

            //_context.thuocEntities.Update(existingMaThuoc);

            //_context.Entry(existingCTXuatThuoc).CurrentValues.SetValues(entity);
            //await _context.SaveChangesAsync();
        }

    }
}
