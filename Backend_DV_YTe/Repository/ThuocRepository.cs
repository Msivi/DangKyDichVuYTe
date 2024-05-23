using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace Backend_DV_YTe.Repository
{
    public class ThuocRepository:IThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ThuocRepository(AppDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;

        }
        public async Task<string> CreateThuoc(ThuocEntity entity)
        {
            var existingThuoc = await _context.thuocEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            if (existingThuoc != null)
            {
                throw new Exception(message: "Id is already exist!");
            }

            var mapEntity = _mapper.Map<ThuocEntity>(entity);

            await _context.thuocEntities.AddAsync(mapEntity);
            await _context.SaveChangesAsync();

            return mapEntity.Id.ToString();
        }

        public async Task<ThuocEntity> DeleteThuoc(int id, bool isPhysical)
        {
            try
            {
                var entity = await _context.thuocEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
                if (entity == null)
                {
                    throw new Exception("Không tìm thấy nhà cung cấp!");
                }
                else
                {
                    if (isPhysical)
                    {
                        _context.thuocEntities.Remove(entity);
                    }
                    else
                    {
                        entity.DeletedTime = DateTimeOffset.Now;
                        _context.thuocEntities.Update(entity);
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

        public async Task<ICollection<ThuocEntity>> GetAllThuoc()
        {
            try
            {
                var entities = await _context.thuocEntities
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

        public async Task<ICollection<AllThuocModel>> GetThuocByLoaiThuoc(int entity)
        {
            try
            {
                var entities = await _context.thuocEntities
                    .Include(t => t.LoaiThuoc)
                    .Where(c => c.MaLoaiThuoc == entity && c.DeletedTime == null)
                    .Select(t => new AllThuocModel
                    {
                        id = t.Id,
                        tenThuoc = t.tenThuoc,
                        donViTinh = t.donViTinh,
                        donGia = t.donGia,
                        soLuong = t.soLuong,
                        ngaySanXuat = t.ngaySanXuat,
                        ngayHetHan = t.ngayHetHan,
                        nhaSanXuat = t.nhaSanXuat,
                        thanhPhan = t.thanhPhan,
                        tenDanhMuc = t.LoaiThuoc.tenLoaiThuoc,
                        moTa = t.moTa
                    })
                    .ToListAsync();

                if (entities.Count == 0)
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

        public async Task<byte[]> DownloadPdfFile(int entity)
        {
            try
            {
                var danhSachXuatThuoc = await _context.thuocEntities
                    .Include(t => t.LoaiThuoc)
                    .Where(c => c.MaLoaiThuoc == entity && c.DeletedTime == null)
                    .Select(t => new AllThuocModel
                    {
                        id = t.Id,
                        tenThuoc = t.tenThuoc,
                        donViTinh = t.donViTinh,
                        donGia = t.donGia,
                        soLuong = t.soLuong,
                        ngaySanXuat = t.ngaySanXuat,
                        ngayHetHan = t.ngayHetHan,
                        nhaSanXuat = t.nhaSanXuat,
                        thanhPhan = t.thanhPhan,
                        tenDanhMuc = t.LoaiThuoc.tenLoaiThuoc
                    })
                    .ToListAsync();

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
                        PdfPTable table = new PdfPTable(10); // 10 cột trong bảng

                        // Thiết lập font cho tiêu đề cột
                        BaseFont baseFont = BaseFont.CreateFont("D:\\DoAnKySu\\fontVN\\vps-thanh-hoa.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        Font columnHeaderFont = new Font(baseFont, 12, Font.BOLD);

                        // Thêm tiêu đề cột vào bảng
                        table.AddCell(new PdfPCell(new Phrase("Mã thuốc", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Tên thuốc", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Đơn vị tính", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Đơn giá", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Số lượng", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Ngày sản xuất", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Ngày hết hạn", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Nhà sản xuất", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Thành phần", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Tên danh mục", columnHeaderFont)));

                        // Sử dụng danh sách để tạo PDF
                        foreach (var entity1 in danhSachXuatThuoc)
                        {
                            table.AddCell(new PdfPCell(new Phrase(entity1.id.ToString())));
                            table.AddCell(new PdfPCell(new Phrase(entity1.tenThuoc)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.donViTinh)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.donGia.ToString())));
                            table.AddCell(new PdfPCell(new Phrase(entity1.soLuong.ToString())));
                            table.AddCell(new PdfPCell(new Phrase(entity1.ngaySanXuat.ToString())));
                            table.AddCell(new PdfPCell(new Phrase(entity1.ngayHetHan.ToString())));
                            table.AddCell(new PdfPCell(new Phrase(entity1.nhaSanXuat)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.thanhPhan)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.tenDanhMuc)));
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

        public async Task<ThuocEntity> GetThuocById(int id)
        {
            var entity = await _context.thuocEntities.FirstOrDefaultAsync(e => e.Id == id && e.DeletedTime == null);
            if (entity is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entity;
        }

        public async Task<ICollection<ThuocEntity>> SearchThuoc(string searchKey)
        {
            var ListKH = await _context.thuocEntities.ToListAsync();

            // Filter the list and materialize the results
            var filteredList = ListKH.Where(c => (
                c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.tenThuoc.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.nhaSanXuat.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase)
            ) && c.DeletedTime == null).ToList();

            return filteredList;
        }

        public async Task UpdateThuoc(int id, ThuocEntity entity)
        {

            if (entity == null)
            {
                throw new Exception(message: "The entity field is required!");
            }

            var existingThuoc = await _context.thuocEntities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);

            if (existingThuoc == null)
            {
                throw new Exception(message: "Không tìm thấy!");
            }
            existingThuoc.tenThuoc = entity.tenThuoc;
            existingThuoc.donViTinh = entity.donViTinh;
            existingThuoc.donGia = entity.donGia;
            existingThuoc.soLuong = entity.soLuong;
            existingThuoc.ngaySanXuat = entity.ngaySanXuat;
            existingThuoc.ngayHetHan = entity.ngayHetHan;
            existingThuoc.nhaSanXuat = entity.nhaSanXuat;
            existingThuoc.MaLoaiThuoc = entity.MaLoaiThuoc;
            existingThuoc.thanhPhan = entity.thanhPhan;
            existingThuoc.moTa = entity.moTa;
            _context.thuocEntities.Update(existingThuoc);
            await _context.SaveChangesAsync();
        }
    }
}
