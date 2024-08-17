using AutoMapper;
using GarmentFactory.Repository.Entities;
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
