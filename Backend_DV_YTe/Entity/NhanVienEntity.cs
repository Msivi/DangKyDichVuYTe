using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    [Table("NhanVien")]
    public class NhanVienEntity : Entity
    {
        public string tenNhanVien { get; set; }
        public string? Avatar { get; set; }
        public string email { get; set; }
        public string matKhau { get; set; }
        public string SDT { get; set; }
        public string CMND { get; set; }
        public string Role { get; set; }

        //public virtual ICollection<CTMuaThuocEntity> CTMuaThuoc { get; set; }
        //public virtual ICollection<DanhGiaEntity> DanhGia { get; set; }
        public virtual ICollection<KetQuaDichVuEntity> KetQuaDV { get; set; }
        public virtual ICollection<NhapThietBiYTeEntity> NhapThietBiYTe { get; set; }
        //public virtual ICollection<XuatThietBiYTeEntity> XuatThietBiYTe { get; set; }
        public virtual ICollection<NhapThuocEntity> NhapThuoc { get; set; }
        //public virtual ICollection<XuatThuocEntity> XuatThuoc { get; set; }



    }
}
