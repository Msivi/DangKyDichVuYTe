using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Mapper
{
    public class ThuocProfile:Profile
    {
        public ThuocProfile()
        {
            CreateMap<ThuocEntity, ThuocModel>()
                .ReverseMap();
        }
    }
}
