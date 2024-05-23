using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Mapper
{
    public class DichVuProfile:Profile
    {
        public DichVuProfile()
        {
            CreateMap<DichVuEntity, DichVuModel>()
                .ReverseMap();
        }
    }
}
