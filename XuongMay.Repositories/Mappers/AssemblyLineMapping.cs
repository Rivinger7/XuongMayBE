using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.ModelViews.AssemblyLineModelView;
using XuongMay.ModelViews.AssemblyLineModelViews;

namespace XuongMay.Repositories.Mappers
{
    public class AssemblyLineMapping : Profile
    {
        public AssemblyLineMapping()
        {
            CreateMap<AssemblyLine, AssemblyLineModelView>()
                // Map User.FullName to ManagerFullName
                .ForMember(dest => dest.ManagerFullName, opt => opt.MapFrom(src => src.User.FullName))
                // Format CreatedTime
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime.ToString("HH:mm dd/MM/yyyy")))
                // Format LastUpdateTime
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.MapFrom(src => src.LastUpdatedTime.HasValue ? src.LastUpdatedTime.Value.ToString("HH:mm dd/MM/yyyy") : string.Empty))
                // Format DeletedTime
                .ForMember(dest => dest.DeletedTime, opt => opt.MapFrom(src => src.DeletedTime.HasValue ? src.DeletedTime.Value.ToString("HH:mm dd/MM/yyyy") : string.Empty))
                .ReverseMap();
            CreateMap<AssemblyLine, AssemblyLineSummaryModel>()
                // Map User.FullName to ManagerFullName
                .ForMember(dest => dest.ManagerFullName, opt => opt.MapFrom(src => src.User.FullName))
                .ReverseMap();
        }

    }
}
