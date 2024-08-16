using AutoMapper;
using GarmentFactory.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.Repositories.UOW;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Services.Service
{
    public class AuthenticationService : IAuthencationService
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public AuthenticationService(UserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseModel> AuthenticateUserAsync(LoginModelView loginModelView)
        {
            var user = await _userRepository.GetByUsernameAsync(loginModelView.Username);
            if (!BCrypt.Net.BCrypt.Verify(loginModelView.Password, user.Password))
            {
                throw new ArgumentException("Username or Password is incorrect", "loginFail");
            }

            UserResponseModel userModel = _mapper.Map<User, UserResponseModel>(user);

            return userModel;
        }

        public async Task RegisterUserAsync(RegisterModelView registerModelView)
        {
            try
            {
                string username = registerModelView.Username;
                string password = registerModelView.Password;

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                if (await IsUserExistsByUsername(username))
                {
                    throw new ArgumentException("Username already exists", "usernameExists");
                }

                // JWT

                // New User Object
                User newUser = new()
                {
                    Username = username,
                    Password = passwordHash,
                    Role = "Manager",
                    CreatedTime = TimeHelper.GetUtcPlus7Time(),
                    FullName = registerModelView.FullName,
                    IsDeleted = false
                };

                await _userRepository.CreateAsync(newUser);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> IsUserExistsByUsername(string username)
        {
            try
            {
                var existingUser = await _userRepository.GetByUsernameForRegister(username);

                return existingUser is not null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                throw;
            }
        }
    }
}
