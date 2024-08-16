using AutoMapper;
using GarmentFactory.Repository.Context;
using GarmentFactory.Repository.Entities;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.UserModelViews;
using XuongMay.Repositories.UOW;

namespace XuongMay.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, UserRepository userRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseModel>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            IEnumerable<UserResponseModel> userModel = _mapper.Map<IEnumerable<User>, IEnumerable<UserResponseModel>>(users);

            return userModel;
        }

        public Task<UserResponseModel> GetUserAsync(UserResponseModel userModel)
        {
            throw new NotImplementedException();
        }

        public async Task<UserResponseModel> GetUserByIDAsync(int id)
        {
            var user = await _userRepository.GetByIDAsync(id);
            UserResponseModel userModel = _mapper.Map<User, UserResponseModel>(user);

            return userModel;
        }

        public async Task<UserResponseModel> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            UserResponseModel userModel = _mapper.Map<User, UserResponseModel>(user);

            return userModel;
        }
    }
}
