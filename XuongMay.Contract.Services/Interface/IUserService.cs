using XuongMay.Core;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<BasePaginatedList<UserSummaryModel>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<BasePaginatedList<UserSummaryModel>> GetAllAdminsAsync(int pageNumber, int pageSize);
        Task<BasePaginatedList<UserSummaryModel>> GetAllManagersAsync(int pageNumber, int pageSize);
        Task<UserResponseModel> GetUserByIDAsync(int id);
        Task<BasePaginatedList<UserSummaryModel>> GetAvailableManagersAsync(int pageNumber, int pageSize);
        Task<BasePaginatedList<UserSummaryModel>> GetUsersByFilteringAsync(string? fullName, string? role, int pageNumber, int pageSize);
        Task UpdatePasswordAsync(int id, string newPassword, string newConfirmPassword);
        Task UpdateFullNameAsync(int id, string newFullName);
        Task DeleteUserByIDAsync(int id);
        Task DeleteUserByUsernameAsync(string username);
    }
}
