using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Mapper
{
    public class DiaChiProfile:Profile
    {
        public DiaChiProfile()
        {
            CreateMap<DiaChiEntity, DiaChiModel>()
                .ReverseMap();
        }
    }
}
