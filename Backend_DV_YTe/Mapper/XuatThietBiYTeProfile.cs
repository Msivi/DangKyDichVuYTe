﻿using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;

namespace Backend_DV_YTe.Mapper
{
    public class XuatThietBiYTeProfile:Profile
    {
        public XuatThietBiYTeProfile()
        {
            CreateMap<XuatThietBiYTeEntity, XuatThietBiYTeModel>()
                .ReverseMap();
        }
    }
}