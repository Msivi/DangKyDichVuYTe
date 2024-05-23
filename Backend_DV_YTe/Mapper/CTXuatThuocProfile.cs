using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Mapper
{
    public class CTXuatThuocProfile:Profile
    {
        public CTXuatThuocProfile()
        {
            CreateMap<CTXuatThuocEntity, CTXuatThuocModel>()
                .ReverseMap();
        }
    }
}
