using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        //Tasks<IList<UserResponseModel>> GetAll();
        Task<IEnumerable<UserResponseModel>> GetAllUsersAsync();
        Task<IEnumerable<UserResponseModel>> GetAllAdminsAsync();
        Task<IEnumerable<UserResponseModel>> GetAllManagersAsync();
        Task<UserResponseModel> GetUserByIDAsync(int id);
        Task<IEnumerable<UserResponseModel>> GetUsersAsync(string? username, string? fullName, string? role);
        Task UpdatePasswordAsync(int id, string newPassword);
        Task UpdateFullNameAsync(int id, string newFullName);
        Task DeleteUserByIDAsync(int id);
        Task DeleteUserByUsernameAsync(string username);
    }
}
