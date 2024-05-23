using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("Thuoc")]
    public class ThuocEntity:Entity
    {
        public string tenThuoc { get; set; }
        public string donViTinh { get; set; }
        public double donGia { get; set; }
        public int soLuong { get; set; }
        public DateTime ngaySanXuat { get; set; }
        public DateTime ngayHetHan { get; set; }
        public string nhaSanXuat { get; set; }
        public string? thanhPhan { get; set; }
        public string? moTa { get; set; }

        [ForeignKey("LoaiThuoc")]
        public int MaLoaiThuoc { get; set; }
        public virtual LoaiThuocEntity LoaiThuoc { get; set; }

        public virtual ICollection<CTMuaThuocEntity> CTMuaThuoc { get; set; }
        public virtual ICollection<CTNhapThuocEntity> CTNhapThuoc { get; set; }
        public virtual ICollection<CTXuatThuocEntity> CTXuatThuoc { get; set; }
    }
}
