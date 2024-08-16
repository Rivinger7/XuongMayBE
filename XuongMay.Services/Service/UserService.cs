using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IList<UserResponseModel>> GetAll()
        {
            IList<UserResponseModel> users = new List<UserResponseModel>
            {
                new UserResponseModel { Id = "1" },
                new UserResponseModel { Id = "2" },
                new UserResponseModel { Id = "3" }
            };

            return Task.FromResult(users);
        }

        public Task<object> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetUserAsync(object user)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetUserByIDAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}
