using XuongMay.ModelViews.AssemblyLineModelView;
using XuongMay.ModelViews.AssemblyLineModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IAssemblyLineService
    {
        Task<IEnumerable<AssemblyLineModelView>> GetAllAssemblyLineAsync();
        Task<AssemblyLineModelView> GetAssemblyLineByIDAsync(int id);
        Task<AssemblyLineModelView> GetAssemblyLineByManagerIDAsync(int managerID);
        Task<IEnumerable<AssemblyLineModelView>> GetAssemblyLinesByFilteringAsync(string? assemblyLineName, string? description);
        Task<IEnumerable<AssemblyLineModelView>> GetAssemblyLinesByCreatorAsync(string creator);
        Task CreateAssemblyLineAsync(AssemblyLineCreateModel assemblyLineModel);
        Task UpdateAssemblyLineAsync(int id, AssemblyLineUpdateModel assemblyLineModel);
        Task DeleteAssemblyLineByIDAsync(int id);
        Task DeleteAssemblyLineByNameAsync(string assemblyLineName);
    }
}
