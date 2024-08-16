using AutoMapper;
using GarmentFactory.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Repositories.Mappers
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserResponseModel>().ReverseMap();

        }
    }
}
