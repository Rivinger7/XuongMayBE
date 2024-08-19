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
                // Map User.FullName to ManagerFullName
                .ForMember(dest => dest.ManagerFullName, opt => opt.MapFrom(src => src.User.FullName)) 
                .ReverseMap();
        }

    }
}
