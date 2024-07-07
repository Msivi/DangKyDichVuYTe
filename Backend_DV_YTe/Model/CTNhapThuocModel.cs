using Backend_DV_YTe.Entity;
using Org.BouncyCastle.Asn1.X509;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class CTNhapThuocModel
    {
        
        public int MaThuoc { get; set; }
        
        public int MaNhapThuoc { get; set; }
        
        public int soLuong { get; set; }
        public double donGiaNhap { get; set; }
    }
}
