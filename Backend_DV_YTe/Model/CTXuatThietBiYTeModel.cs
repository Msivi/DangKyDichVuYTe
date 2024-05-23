using Backend_DV_YTe.Entity;
using Org.BouncyCastle.Asn1.X509;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class CTXuatThietBiYTeModel
    {
        public int MaXuatThietBiYTe { get; set; }
        public int MaThietBiYTe { get; set; }
        public int soLuong { get; set; }
        public DateTime ngayTao { get; set; }
    }
}
