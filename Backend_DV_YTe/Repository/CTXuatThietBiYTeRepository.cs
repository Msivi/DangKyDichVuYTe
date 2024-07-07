using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;

namespace Backend_DV_YTe.Repository
{
    public class CTXuatThietBiYTeRepository:ICTXuatThietBiYTeRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CTXuatThietBiYTeRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
       public static List<CTXuatThietBiYTeEntity> danhSachXuatThietBi = new List<CTXuatThietBiYTeEntity>();

        public async Task<string> CreateCTXuatThietBiYTe(CTXuatThietBiYTeEntity entity)
        {
            var existingCTXuatThietBiYTe = await _context.cTXuatThietBiYTeEntities
                .FirstOrDefaultAsync(c => c.MaThietBiYTe == entity.MaThietBiYTe && c.MaXuatThietBiYTe == entity.MaXuatThietBiYTe);
            var existingSLThietBi = await _context.thietBiYTeEntities
                   .FirstOrDefaultAsync(c => c.Id == entity.MaThietBiYTe && c.DeletedTime == null);

            //if (existingSLThietBi.soLuong < entity.soLuong)
            //{
            //    throw new Exception($"Không đủ số lượng thiết bị để xuất! (Số lượng hiện tại trong kho: {existingSLThietBi.soLuong})");
            //}

            //if (existingCTXuatThietBiYTe != null)
            //{
            //    existingCTXuatThietBiYTe.soLuong += entity.soLuong;
            //    _context.cTXuatThietBiYTeEntities.Update(existingCTXuatThietBiYTe);

            //    var existingMaThietBi = await _context.thietBiYTeEntities
            //        .FirstOrDefaultAsync(c => c.Id == entity.MaThietBiYTe && c.DeletedTime == null);
            //    if (existingMaThietBi != null)
            //    {
            //        existingMaThietBi.soLuong -= entity.soLuong;
            //        _context.thietBiYTeEntities.Update(existingMaThietBi);
            //    }
            //}
            //else
            //{
            //    var existingMaThietBi = await _context.thietBiYTeEntities
            //        .FirstOrDefaultAsync(c => c.Id == entity.MaThietBiYTe && c.DeletedTime == null);
            //    if (existingMaThietBi == null)
            //    {
            //        throw new Exception(message: "Mã thuoc không tồn tại!");
            //    }

            //    var existingMaXuat = await _context.xuatThuocEntities
            //        .FirstOrDefaultAsync(c => c.Id == entity.MaXuatThietBiYTe && c.DeletedTime == null);
            //    if (existingMaXuat == null)
            //    {
            //        throw new Exception(message: "Mã xuất thiết bị không tồn tại!");
            //    }

            //    //if (existingMaThietBi.soLuong < entity.soLuong)
            //    //{
            //    //    throw new Exception($"Không đủ số lượng thiết bị để xuất! (Số lượng hiện tại trong kho: {existingMaThietBi.soLuong})");
            //    //}


            //    existingMaThietBi.soLuong -= entity.soLuong;
            //    _context.thietBiYTeEntities.Update(existingMaThietBi);

            //    // Thêm phiếu xuất vaccine mới
            //    var mapEntity = _mapper.Map<CTXuatThietBiYTeEntity>(entity);
            //    await _context.cTXuatThietBiYTeEntities.AddAsync(mapEntity);
            //}

            //await _context.SaveChangesAsync();
            //danhSachXuatThietBi.Add(entity);

            return $"{entity.MaXuatThietBiYTe}-{entity.MaThietBiYTe}";
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
                    table.AddCell(new PdfPCell(new Phrase("Mã thiết bị", columnHeaderFont)));
                    table.AddCell(new PdfPCell(new Phrase("Mã xuất thiết bị", columnHeaderFont)));
                    table.AddCell(new PdfPCell(new Phrase("Số lượng", columnHeaderFont)));
                    //table.AddCell(new PdfPCell(new Phrase("Ngày tạo", columnHeaderFont)));

                    if (danhSachXuatThietBi is null || danhSachXuatThietBi.Count == 0)
                    {
                        throw new Exception("Danh sách xuất thiết bị rỗng!");
                    }

                    // Sử dụng danh sách từ list để tạo PDF
                    foreach (var entity in danhSachXuatThietBi)
                    {
                        table.AddCell(new PdfPCell(new Phrase(entity.MaThietBiYTe.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(entity.MaXuatThietBiYTe.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(entity.soLuong.ToString())));
                        //table.AddCell(new PdfPCell(new Phrase(entity.ngayTao.ToString())));
                    }

                    // Thêm bảng vào tệp tin PDF
                    document.Add(table);

                    document.Close();

                    // Chuyển đổi MemoryStream thành mảng byte
                    byte[] bytes = memoryStream.ToArray();

                    
                    danhSachXuatThietBi.Clear();

                    // Trả về mảng byte
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

            var CTXuatThietBiYTes = _context.cTXuatThietBiYTeEntities
                .Where(c => c.CreateTimes >= startTime.Date && c.CreateTimes <= endTime.Date)
                .ToList();

            var vaccineReport = new Dictionary<int, int>();

            foreach (var CTXuatThietBiYTe in CTXuatThietBiYTes)
            {
                int MaThuoc = CTXuatThietBiYTe.MaThietBiYTe;

                if (vaccineReport.ContainsKey(MaThuoc))
                {
                    vaccineReport[MaThuoc] += CTXuatThietBiYTe.soLuong;
                }
                else
                {
                    vaccineReport[MaThuoc] = CTXuatThietBiYTe.soLuong;
                }
            }

            return vaccineReport;
        }

        public async Task<CTXuatThietBiYTeEntity> DeleteCTXuatThietBiYTe(int MaThuoc, int MaXuatThuoc, bool isPhysical)
        {
            try
            {
                var entity = await _context.cTXuatThietBiYTeEntities.FirstOrDefaultAsync(e => e.MaThietBiYTe == MaThuoc && e.MaXuatThietBiYTe == MaXuatThuoc && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy Id chi tiết xuất thiết bị!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.cTXuatThietBiYTeEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.cTXuatThietBiYTeEntities.Update(entity);
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

        public async Task<ICollection<CTXuatThietBiYTeEntity>> GetAllCTXuatThietBiYTe()
        {
            try
            {
                var entities = await _context.cTXuatThietBiYTeEntities
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

        public async Task<CTXuatThietBiYTeEntity> GetCTXuatThietBiYTeById(int MaThuoc, int MaXuatThuoc)
        {
            var entity = await _context.cTXuatThietBiYTeEntities.FirstOrDefaultAsync(e => e.MaThietBiYTe == MaThuoc && e.MaXuatThietBiYTe == MaXuatThuoc && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task UpdateCTXuatThietBiYTe(int MaThuoc, int MaXuatThuoc, CTXuatThietBiYTeEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "Thông tin cập nhật rỗng!");
            }

            var existingCTXuatThietBiYTe = await _context.cTXuatThietBiYTeEntities
                .FirstOrDefaultAsync(e => e.MaThietBiYTe == MaThuoc && e.MaXuatThietBiYTe == MaXuatThuoc && e.DeletedTime == null);
            if (existingCTXuatThietBiYTe == null)
            {
                throw new Exception(message: "Không tìm thấy chi tiết xuất thiết bị!");
            }

            var existingMaThietBi = await _context.thietBiYTeEntities
                .FirstOrDefaultAsync(c => c.Id == MaThuoc && c.DeletedTime == null);
            if (existingMaThietBi == null)
            {
                throw new Exception(message: "Mã thiết bị không tồn tại!");
            }

            var existingMaXuat = await _context.xuatThuocEntities
                .FirstOrDefaultAsync(c => c.Id == MaXuatThuoc && c.DeletedTime == null);
            if (existingMaXuat == null)
            {
                throw new Exception(message: "Mã xuất thiết bị không tồn tại!");
            }

            //if (existingCTXuatThietBiYTe.soLuong > entity.soLuong)
            //{
            //    existingMaThietBi.soLuong = (existingMaThietBi.soLuong != null ? existingMaThietBi.soLuong : 0)

            //        + (existingCTXuatThietBiYTe.soLuong - entity.soLuong);

            //}
            //else
            //{
            //    if (existingMaThietBi.soLuong < (entity.soLuong - existingCTXuatThietBiYTe.soLuong))
            //    {
            //        throw new Exception(message: $"Số lượng hiện tại trong kho không đủ để thực hiện cập nhật này! Số lượng hiện tại kho đang là: {existingMaThietBi.soLuong}");
            //    }
            //    else
            //    {
            //        existingMaThietBi.soLuong = (existingMaThietBi.soLuong != null ? existingMaThietBi.soLuong : 0)

            //            - (entity.soLuong - existingCTXuatThietBiYTe.soLuong);
            //    }

            //}

            //_context.thietBiYTeEntities.Update(existingMaThietBi);

            //_context.Entry(existingCTXuatThietBiYTe).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }
    }
}
