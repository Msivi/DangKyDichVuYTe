using Backend_DV_YTe.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_DV_YTe.Model
{
    public class CTBacSiModel
    {
        public int MaBacSi { get; set; }
        public int MaChuyenKhoa { get; set; }
    }
}
