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
    }
}
