using Backend_DV_YTe.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class CTMuaThietBiYTeModel
    {
        public int MaHoaDon { get; set; }
        public int MaThietBiYTe { get; set; }
        public int soLuong { get; set; }
        
        
    }
}
