using AutoMapper;
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

        public async Task<IEnumerable<UserResponseModel>> GetAllAdminsAsync()
        {
            var users = await _userRepository.GetAllAdminsAsync();
            IEnumerable<UserResponseModel> userModel = _mapper.Map<IEnumerable<User>, IEnumerable<UserResponseModel>>(users);

            return userModel;
        }

        public async Task<IEnumerable<UserResponseModel>> GetAllManagersAsync()
        {
            var users = await _userRepository.GetAllManagersAsync();
            IEnumerable<UserResponseModel> userModel = _mapper.Map<IEnumerable<User>, IEnumerable<UserResponseModel>>(users);

            return userModel;
        }

        public async Task<UserResponseModel> GetUserByIDAsync(int id)
        {
            var user = await _userRepository.GetByIDAsync(id);
            UserResponseModel userModel = _mapper.Map<User, UserResponseModel>(user);

            return userModel;
        }

        public async Task<IEnumerable<UserResponseModel>> GetUsersAsync(string? username, string? fullName, string? role)
        {
            var users = await _userRepository.GetUsersAsync(username, fullName, role);

            IEnumerable<UserResponseModel> userModel = _mapper.Map<IEnumerable<User>, IEnumerable<UserResponseModel>>(users);

            return userModel;
        }

        public async Task UpdatePasswordAsync(int id, string newPassword)
        {
            var user = await _userRepository.GetByIDAsync(id);

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.Password = newPasswordHash;

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateFullNameAsync(int id, string newFullName)
        {
            var user = await _userRepository.GetByIDAsync(id);

            user.FullName = newFullName.Trim();

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserByIDAsync(int id)
        {
            var user = await _userRepository.GetByIDAsync(id);

            await _userRepository.DeleteAsync(user);
        }

        public async Task DeleteUserByUsernameAsync(string username)
        {
            var userModel = await _userRepository.GetByUsernameAsync(username);

            await _userRepository.DeleteAsync(userModel);
        }
    }
}
