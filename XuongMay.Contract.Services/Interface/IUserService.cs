using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseModel>> GetAllUsersAsync();
        Task<IEnumerable<UserResponseModel>> GetAllAdminsAsync();
        Task<IEnumerable<UserResponseModel>> GetAllManagersAsync();
        Task<UserResponseModel> GetUserByIDAsync(int id);
        Task<IEnumerable<UserResponseModel>> GetUsersByFilteringAsync(string? username, string? fullName, string? role);
        Task UpdatePasswordAsync(int id, string newPassword, string newConfirmPassword);
        Task UpdateFullNameAsync(int id, string newFullName);
        Task DeleteUserByIDAsync(int id);
        Task DeleteUserByUsernameAsync(string username);
    }
}
