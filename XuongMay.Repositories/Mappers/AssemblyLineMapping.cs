using AutoMapper;
using GarmentFactory.Repository.Entities;
using XuongMay.ModelViews.AssemblyLineModelView;

namespace XuongMay.Repositories.Mappers
{
    public class AssemblyLineMapping : Profile
    {
        public AssemblyLineMapping()
        {
            CreateMap<AssemblyLine, AssemblyLineModelView>().ReverseMap();
        }

    }
}
