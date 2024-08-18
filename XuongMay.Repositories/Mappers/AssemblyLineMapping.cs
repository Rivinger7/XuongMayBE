using AutoMapper;
using GarmentFactory.Repository.Entities;
using XuongMay.ModelViews.AssemblyLineModelView;

namespace XuongMay.Repositories.Mappers
{
    public class AssemblyLineMapping : Profile
    {
        public AssemblyLineMapping()
        {
            CreateMap<AssemblyLine, AssemblyLineModelView>()
                .ForMember(dest => dest.ManagerFullName, opt => opt.MapFrom(src => src.User.FullName)) // Map User.FullName to ManagerFullName
                .ReverseMap();
        }

    }
}
