using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Mapper
{
    public class NhapThuocProfile:Profile
    {
        public NhapThuocProfile()
        {
            CreateMap<NhapThuocEntity, NhapThuocModel>()
                .ReverseMap();
        }
    }
}
