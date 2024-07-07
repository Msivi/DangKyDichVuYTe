using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using static iTextSharp.text.pdf.events.IndexEvents;
using Microsoft.AspNetCore.Hosting;
using static iText.IO.Util.IntHashtable;

namespace Backend_DV_YTe.Repository
{
    public class ThuocRepository:IThuocRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ThuocRepository(AppDbContext Context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = Context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> CreateThuoc(ThuocEntity entity, IFormFile? imageFile)
        {
            var existingThuoc = await _context.thuocEntities
              .FirstOrDefaultAsync(c => c.Id == entity.Id && (c.DeletedTime == null || c.DeletedTime != null));

            //if (entity.ngayHetHan<= entity.ngaySanXuat)
            //{
            //    throw new Exception(message: "Ngày hết hạng phải lớn hơn ngày sản xuất!");
            //}
            //if (existingThuoc != null)
            //{
            //    throw new Exception(message: "Id is already exist!");
            //}
            if(imageFile !=null)
            {
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                entity.hinhAnh = $"/Images/" + uniqueFileName;
            }    
            //else
            //{
            //    entity.hinhAnh = null;
            //}    
            

            var mapEntity = _mapper.Map<ThuocEntity>(entity);
            mapEntity.donGia = 0;
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

        public async Task<ICollection<AllThuocModel>> GetAllThuoc()
        {
            try
            {
                var entities = await _context.thuocEntities
                     .Where(c => c.DeletedTime == null)
                     .Select(t => new AllThuocModel
                     {
                         id = t.Id,
                         tenThuoc = t.tenThuoc,
                         donViTinh = t.donViTinh,
                         donGia = t.donGia,
                         soLuong = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                         ngaySanXuat = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                         ngayHetHan = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                         nhaSanXuat = t.nhaSanXuat,
                         thanhPhan = t.thanhPhan,
                         tenDanhMuc = t.LoaiThuoc.tenLoaiThuoc,
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
        
        //public async Task<ICollection<AllThuocModel>> GetThuocByLoaiThuoc(int entity)
        //{
        //    try
        //    {
        //        var entities = await _context.thuocEntities
        //            .Include(t => t.LoaiThuoc)
        //            .Where(c => c.MaLoaiThuoc == entity && c.DeletedTime == null)
        //            .Select(t => new AllThuocModel
        //            {
        //                id = t.Id,
        //                tenThuoc = t.tenThuoc,
        //                donViTinh = t.donViTinh,
        //                donGia = t.donGia,
        //                soLuong = t.soLuong,
        //                //ngaySanXuat = t.ngaySanXuat,
        //                //ngayHetHan = t.ngayHetHan,
        //                nhaSanXuat = t.nhaSanXuat,
        //                thanhPhan = t.thanhPhan,
        //                tenDanhMuc = t.LoaiThuoc.tenLoaiThuoc,
        //                moTa = t.moTa,
        //                hinhAnh=t.hinhAnh
        //            })
        //            .ToListAsync();

        //        if (entities.Count == 0)
        //        {
        //            throw new Exception("Empty list!");
        //        }

        //        return entities;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<AllThuocModel> GetThuocById(int id)
        {
            var entities = await _context.thuocEntities
            //.Include(t => t.LoaiThuoc)
                    .Where(c => c.Id == id && c.DeletedTime == null)
                    .Select(t => new AllThuocModel
                    {
                        id = t.Id,
                        tenThuoc = t.tenThuoc,
                        donViTinh = t.donViTinh,
                        donGia = t.donGia,
                        soLuong = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                        ngaySanXuat = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                        ngayHetHan = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                        nhaSanXuat = t.nhaSanXuat,
                        thanhPhan = t.thanhPhan,
                        tenDanhMuc = t.LoaiThuoc.tenLoaiThuoc,
                        moTa = t.moTa,
                        hinhAnh = t.hinhAnh
                    }).FirstOrDefaultAsync();
            if (entities is null)
            {
                throw new Exception("not found or already deleted.");
            }
            return entities;
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
                        soLuong = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                        ngaySanXuat = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                        ngayHetHan = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                        nhaSanXuat = t.nhaSanXuat,
                        thanhPhan = t.thanhPhan,
                        tenDanhMuc = t.LoaiThuoc.tenLoaiThuoc,
                        moTa = t.moTa,
                        hinhAnh = t.hinhAnh
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
                    .Include(l=>l.LoThuoc)
                    .Include(t => t.LoaiThuoc)
                    .Where(c => c.MaLoaiThuoc == entity && c.DeletedTime == null)
                    .Select(t => new AllThuocModel
                    {
                        id = t.Id,
                        tenThuoc = t.tenThuoc,
                        donViTinh = t.donViTinh,
                        donGia = t.donGia,
                        soLuong = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                        ngaySanXuat = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                        ngayHetHan = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                        nhaSanXuat = t.nhaSanXuat,
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
                        PdfPTable table = new PdfPTable(9); // 10 cột trong bảng
                        float[] columnWidths = new float[] { 1f, 3.5f, 0.5f, 1.5f, 1f, 1.5f, 1.5f, 1.5f, 2.5f };
                        table.SetWidths(columnWidths);

                        string fontRelativePath = Path.Combine("arial.ttf");
                        string fontAbsolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fontRelativePath);

                        // Thiết lập font cho tiêu đề cột
                        BaseFont baseFont = BaseFont.CreateFont(fontAbsolutePath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        Font columnHeaderFont = new Font(baseFont, 12, Font.BOLD);
                        Font columnHeaderFontnd = new Font(baseFont, 8);
                        // Thêm tiêu đề cột vào bảng
                        table.AddCell(new PdfPCell(new Phrase("Mã", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Tên thuốc", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("DVT", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("giá", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("SL", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("NSX", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("NHH", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Nhà sản xuất", columnHeaderFont)));
                        table.AddCell(new PdfPCell(new Phrase("Tên danh mục", columnHeaderFont)));

                         
                        // Sử dụng danh sách để tạo PDF
                        foreach (var entity1 in danhSachXuatThuoc)
                        {
                            table.AddCell(new PdfPCell(new Phrase(entity1.id.ToString(),columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.tenThuoc, columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.donViTinh, columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.donGia.ToString(), columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.soLuong.ToString(), columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.ngaySanXuat.ToString(), columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.ngayHetHan.ToString(), columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.nhaSanXuat, columnHeaderFontnd)));
                            table.AddCell(new PdfPCell(new Phrase(entity1.tenDanhMuc, columnHeaderFontnd)));
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



        public async Task<ICollection<AllThuocModel>> SearchThuoc(string searchKey)
        {
            try
            {
                // Lấy danh sách các thuốc từ database
                var query = _context.thuocEntities
                    .Include(t => t.LoaiThuoc)
                    .Where(c => c.DeletedTime == null);

                // Chuyển đổi dữ liệu sang bộ nhớ
                var entities = await query.ToListAsync();

                // Áp dụng tìm kiếm bằng cách sử dụng LINQ to Objects
                var filteredEntities = entities
                    .Where(c => c.Id.ToString().Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                                c.tenThuoc.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                                (c.nhaSanXuat != null && c.nhaSanXuat.Contains(searchKey, StringComparison.OrdinalIgnoreCase)))
                    .Select(t => new AllThuocModel
                    {
                        id = t.Id,
                        tenThuoc = t.tenThuoc,
                        donViTinh = t.donViTinh,
                        donGia = t.donGia,
                        soLuong = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .Sum(l => l.soLuong),
                        ngaySanXuat = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngaySanXuat)
                            .FirstOrDefault(),
                        ngayHetHan = _context.loThuocEntities
                            .Where(l => l.MaThuoc == t.Id && l.DeletedTime == null)
                            .OrderByDescending(l => l.ngaySanXuat)
                            .Select(l => l.ngayHetHan)
                            .FirstOrDefault(),
                        nhaSanXuat = t.nhaSanXuat,
                        thanhPhan = t.thanhPhan,
                        tenDanhMuc = t.LoaiThuoc.tenLoaiThuoc,
                        moTa = t.moTa,
                        hinhAnh = t.hinhAnh
                    })
                    .ToList();

                if (filteredEntities.Count == 0)
                {
                    throw new Exception("Empty list!");
                }

                return filteredEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task UpdateThuoc(int id, ThuocEntity entity, IFormFile? imageFile)
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
            
            if (imageFile != null)
            {
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                existingThuoc.hinhAnh = $"/Images/" + uniqueFileName;
            }
            else
            {
                existingThuoc.hinhAnh = null;
            } 
                

            existingThuoc.tenThuoc = entity.tenThuoc;
            existingThuoc.donViTinh = entity.donViTinh;
            existingThuoc.donGia = entity.donGia;
            //existingThuoc.soLuong = entity.soLuong;
            //existingThuoc.ngaySanXuat = entity.ngaySanXuat;
            //existingThuoc.ngayHetHan = entity.ngayHetHan;
            existingThuoc.nhaSanXuat = entity.nhaSanXuat;
            existingThuoc.MaLoaiThuoc = entity.MaLoaiThuoc;
            existingThuoc.thanhPhan = entity.thanhPhan;
            existingThuoc.moTa = entity.moTa;
            
            _context.thuocEntities.Update(existingThuoc);
            await _context.SaveChangesAsync();
        }
    }
}
