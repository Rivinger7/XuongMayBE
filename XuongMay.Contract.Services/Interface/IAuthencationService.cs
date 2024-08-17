using XuongMay.ModelViews.AuthModelViews;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IAuthencationService
    {
        Task RegisterUserAsync(RegisterModelView registerModelView);
        Task<UserResponseModel> AuthenticateUserAsync(LoginModelView loginModelView);
    }
}
