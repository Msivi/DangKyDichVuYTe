using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Mapper
{
    public class NhaCungCapProfile:Profile
    {
        public NhaCungCapProfile()
        {
            CreateMap<NhaCungCapEntity, NhaCungCapModel>()
                .ReverseMap();
        }
    }
}
