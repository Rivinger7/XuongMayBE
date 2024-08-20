using XuongMay.Core;
using XuongMay.ModelViews.AssemblyLineModelView;
using XuongMay.ModelViews.AssemblyLineModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IAssemblyLineService
    {
        Task<BasePaginatedList<AssemblyLineSummaryModel>> GetAllAssemblyLineAsync(int pageNumber, int pageSize);
        Task<AssemblyLineModelView> GetAssemblyLineByIDAsync(int id);
        Task<AssemblyLineModelView> GetAssemblyLineByManagerNameAsync(string managerName);
        Task<AssemblyLineModelView> GetAssemblyLineByManagerIDAsync(int managerID);
        Task<BasePaginatedList<AssemblyLineSummaryModel>> GetAssemblyLinesByFilteringAsync(string? assemblyLineName, string? creator, int pageNumber, int pageSize);
        Task<BasePaginatedList<AssemblyLineModelView>> GetAssemblyLinesByCreatorAsync(string creator, int pageNumber, int pageSize);
        Task CreateAssemblyLineAsync(AssemblyLineCreateModel assemblyLineModel);
        Task UpdateAssemblyLineAsync(int id, AssemblyLineUpdateModel assemblyLineModel);
        Task DeleteAssemblyLineByIDAsync(int id);
        Task DeleteAssemblyLineByNameAsync(string assemblyLineName);
    }
}
