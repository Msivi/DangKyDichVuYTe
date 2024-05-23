using Backend_DV_YTe.Entity;
using Org.BouncyCastle.Asn1.X509;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class CTXuatThuocModel
    {
       
        public int MaThuoc { get; set; }
       
        public int MaXuatThuoc { get; set; }
       
        public int soLuong { get; set; }
        public DateTime ngayTao { get; set; }
    }
}
