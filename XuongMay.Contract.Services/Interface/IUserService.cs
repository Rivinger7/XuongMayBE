using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        //Tasks<IList<UserResponseModel>> GetAll();
        Task<IEnumerable<UserResponseModel>> GetAllUsersAsync();
        Task<UserResponseModel> GetUserAsync(UserResponseModel userModel);
        Task<UserResponseModel> GetUserByIDAsync(int id);
        Task<UserResponseModel> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserResponseModel>> GetUserByFullNameAsync(string fullName);
        Task<IEnumerable<UserResponseModel>> GetUsersByRoleAsync(string role);
        Task UpdatePasswordAsync(int id, string newPassword);
        Task UpdateFullNameAsync(int id, string newFullName);
        Task DeleteUserByIDAsync(int id);
        Task DeleteUserByUsernameAsync(string username);
    }
}
