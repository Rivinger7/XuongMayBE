using XuongMay.Core;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<BasePaginatedList<UserResponseModel>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<BasePaginatedList<UserResponseModel>> GetAllAdminsAsync(int pageNumber, int pageSize);
        Task<BasePaginatedList<UserResponseModel>> GetAllManagersAsync(int pageNumber, int pageSize);
        Task<UserResponseModel> GetUserByIDAsync(int id);
        Task<BasePaginatedList<UserResponseModel>> GetUsersByFilteringAsync(string? username, string? fullName, string? role, int pageNumber, int pageSize);
        Task UpdatePasswordAsync(int id, string newPassword, string newConfirmPassword);
        Task UpdateFullNameAsync(int id, string newFullName);
        Task DeleteUserByIDAsync(int id);
        Task DeleteUserByUsernameAsync(string username);
    }
}
