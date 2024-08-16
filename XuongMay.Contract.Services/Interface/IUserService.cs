using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        //Task<IList<UserResponseModel>> GetAll();
        Task<object> GetAllUsersAsync();
        Task<object> GetUserAsync(object user);
        Task<object> GetUserByIDAsync(int id);
        Task<object> GetUserByUsernameAsync(string username);
        Task<object> GetUserByEmailAsync(string email);

    }
}
