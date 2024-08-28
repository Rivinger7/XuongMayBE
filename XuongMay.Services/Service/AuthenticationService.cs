using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.Core.Utils;
using XuongMay.Contract.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace XuongMay.Services.Service
{
    public class AuthenticationService : IAuthencationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IUnitOfWork unitOfWork, IMapper mapper, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenticatedResponseModelView> AuthenticateUserAsync(LoginModelView loginModelView)
        {
            // Retrieve user by username from the repository
            User retrieveUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Username == loginModelView.Username) ?? throw new ArgumentException("Username or Password is incorrect");

            // Verify if the password does not match with password hash
            if (!BCrypt.Net.BCrypt.Verify(loginModelView.Password, retrieveUser.Password))
            {
                throw new ArgumentException("Username or Password is incorrect");
            }

            // JWT
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, retrieveUser.Id.ToString()),
                    new Claim(ClaimTypes.Role, retrieveUser.Role)
                };

            //Call method to generate access token
            _jwtService.GenerateAccessToken(claims, retrieveUser.Id, out string accessToken, out string refreshToken);

            // New object ModelView
            AuthenticatedResponseModelView authenticationModel = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            // Store user session
            _httpContextAccessor.HttpContext.Session.SetInt32("userID", retrieveUser.Id);

            return authenticationModel;
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
