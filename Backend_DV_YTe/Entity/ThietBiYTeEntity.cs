using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("ThietBiYTe")]
    public class ThietBiYTeEntity:Entity
    {
        public string tenThietBi { get; set; }
        public double donGia { get; set; }
        //public int soLuong { get; set; }
        //public DateTime ngaySanXuat { get; set; }
        //public DateTime ngayHetHan { get; set; }
        public string nhaSanXuat { get; set; }
        public string? moTa { get; set; }
        public string? hinhAnh { get; set; }


        [ForeignKey("LoaiThietBi")]
        public int MaLoaiThietBi { get; set; }
        public virtual LoaiThietBiEntity LoaiThietBi { get; set; }

        public virtual ICollection<CTNhapThietBiYTeEntity> CTNhapThietBiYTe { get; set; }
        public virtual ICollection<CTXuatThietBiYTeEntity> CTXuatThietBiYTe { get; set; }
        public virtual ICollection<CTMuaThietBiYTeEntity> CTMuaThietBiYTe { get; set; }
        public virtual ICollection<LoThuocEntity> LoThuoc { get; set; }
    }
}
