using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Repositories.Mappers
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserResponseModel>()
                // Map AssemblyLineID: If the user's role is "Manager", map the AssemblyLine ID, otherwise set it to null
                .ForMember(dest => dest.AssemblyLineID, opt => opt.MapFrom(src => src.Role == "Manager" ? src.AssemblyLine.Id : (int?)null))
                // Map AssemblyLineName: If the user's role is "Manager", map the AssemblyLine name, otherwise set it to null
                .ForMember(dest => dest.AssemblyLineName, opt => opt.MapFrom(src => src.Role == "Manager" ? src.AssemblyLine.Name : null)).ReverseMap();

            CreateMap<User, UserSummaryModel>().ReverseMap();
        }
    }
}
