using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.UserModelViews;
using XuongMay.Contract.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace XuongMay.Services.Service
{
    public class AuthenticationService : IAuthencationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserResponseModel> AuthenticateUserAsync(LoginModelView loginModelView)
        {
            // Assign value
            string username = loginModelView.Username;
            string password = loginModelView.Password;

            // Retrieve user by username from the repository
            User retrieveUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Username == username) ?? throw new ArgumentException("Username or Password is incorrect");

            // Verify if the password does not match with password hash
            if (!BCrypt.Net.BCrypt.Verify(password, retrieveUser.Password))
            {
                throw new ArgumentException("Username or Password is incorrect");
            }
            
            // JWT

            // Store user id in Session
            //_httpContextAccessor.HttpContext.Session.SetString("UserID", retrieveUser.Id.ToString());

            // Map the User entities to UserResponseModel
            UserResponseModel userModel = _mapper.Map<User, UserResponseModel>(retrieveUser);

            return userModel;
        }

        public async Task RegisterUserAsync(RegisterModelView registerModelView)
        {
            // Assign value
            string username = registerModelView.Username;
            string password = registerModelView.Password;
            string confirmedPassword = registerModelView.ConfirmedPassword;

            // Check if the password and confirmation password match
            bool isConfirmedPassword = password == confirmedPassword;
            if (!isConfirmedPassword)
            {
                throw new ArgumentException("Password and Confirmed Password do not match");
            }

            // Hash Password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // Check if the user exists
            bool isExistingUser = await _unitOfWork.GetRepository<User>().Entities.AnyAsync(user => user.Username == username);
            if (isExistingUser)
            {
                throw new ArgumentException("Username already exists");
            }

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

            // Add a new user to the repository
            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            await _unitOfWork.SaveAsync();
        }

    }
}
