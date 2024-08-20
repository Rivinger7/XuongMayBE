using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<UserSummaryModel>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            // Retrieve all retrieveUser from the repository
            IEnumerable<User> retrieveUsers = await _unitOfWork.GetRepository<User>().Entities.Where(user => !user.IsDeleted).ToListAsync();

            // Verify if the list of retrieved users is empty
            if (!retrieveUsers.Any())
            {
                throw new ArgumentException("No users found");
            }

            // Get the total number of users
            int totalUsers = retrieveUsers.Count();

            // Pagination
            IReadOnlyCollection<User> pageinatedUsers = retrieveUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Map the User entities to UserResponseModel
            IReadOnlyCollection<UserSummaryModel> userModel = _mapper.Map<IReadOnlyCollection<User>, IReadOnlyCollection<UserSummaryModel>>(pageinatedUsers);

            // Create the paginated list
            BasePaginatedList<UserSummaryModel> paginatedList = new(userModel, totalUsers, pageNumber, pageSize);

            return paginatedList;
        }

        public async Task<BasePaginatedList<UserSummaryModel>> GetAllAdminsAsync(int pageNumber, int pageSize)
        {
            // Retrieve all retrieveUser who have role is Admin from the repository
            IEnumerable<User> retrieveUsers = await _unitOfWork.GetRepository<User>().Entities.Where(user => user.Role == "Admin" && !user.IsDeleted).ToListAsync();

            // Verify if the list of retrieved users is empty
            if (!retrieveUsers.Any())
            {
                throw new ArgumentException("No users found");
            }

            // Get the total number of users
            int totalUsers = retrieveUsers.Count();

            // Pagination
            IReadOnlyCollection<User> pageinatedUsers = retrieveUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Map the User entities to UserResponseModel
            IReadOnlyCollection<UserSummaryModel> userModel = _mapper.Map<IReadOnlyCollection<User>, IReadOnlyCollection<UserSummaryModel>>(pageinatedUsers);

            // Create the paginated list
            BasePaginatedList<UserSummaryModel> paginatedList = new(userModel, totalUsers, pageNumber, pageSize);

            return paginatedList;
        }

        public async Task<BasePaginatedList<UserSummaryModel>> GetAllManagersAsync(int pageNumber, int pageSize)
        {
            // Retrieve all users who have role is Manager from the repository
            IEnumerable<User> retrieveUsers = await _unitOfWork.GetRepository<User>().Entities.Where(user => user.Role == "Manager" && !user.IsDeleted).ToListAsync();

            // Verify if the list of retrieved users is empty
            if (!retrieveUsers.Any())
            {
                throw new ArgumentException("No users found");
            }

            // Get the total number of users
            int totalUsers = retrieveUsers.Count();

            // Pagination
            IReadOnlyCollection<User> pageinatedUsers = retrieveUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Map the User entities to UserResponseModel
            IReadOnlyCollection<UserSummaryModel> userModel = _mapper.Map<IReadOnlyCollection<User>, IReadOnlyCollection<UserSummaryModel>>(pageinatedUsers);

            // Create the paginated list
            BasePaginatedList<UserSummaryModel> paginatedList = new(userModel, totalUsers, pageNumber, pageSize);

            return paginatedList;
        }

        public async Task<UserResponseModel> GetUserByIDAsync(int id)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(id);

            // Retrieves user by ID from the repository
            User retrieveUser = await _unitOfWork.GetRepository<User>().Entities.Include(user => user.AssemblyLine).FirstOrDefaultAsync(user => user.Id == id && !user.IsDeleted) ?? throw new ArgumentException($"User with ID {id} not found");

            // Map the User entities to UserResponseModel
            UserResponseModel userModel = _mapper.Map<User, UserResponseModel>(retrieveUser);

            return userModel;
        }

        public async Task<BasePaginatedList<UserSummaryModel>> GetAvailableManagersAsync(int pageNumber, int pageSize)
        {
            // Retrieves users from the repository
            IEnumerable<User> retrieveUsers = await _unitOfWork.GetRepository<User>().Entities.Include(user => user.AssemblyLine).Where(user => !user.IsDeleted && user.Role == "Manager" && user.AssemblyLine == null).ToListAsync();

            // Get the total number of users
            int totalUsers = retrieveUsers.Count();

            // Pagination
            IReadOnlyCollection<User> pageinatedUsers = retrieveUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Map the User entities to UserResponseModel
            IReadOnlyCollection<UserSummaryModel> userModel = _mapper.Map<IReadOnlyCollection<User>, IReadOnlyCollection<UserSummaryModel>>(pageinatedUsers);

            // Create the paginated list
            BasePaginatedList<UserSummaryModel> paginatedList = new(userModel, totalUsers, pageNumber, pageSize);

            return paginatedList;
        }

        public async Task<BasePaginatedList<UserSummaryModel>> GetUsersByFilteringAsync(string? username, string? fullName, string? role, int pageNumber, int pageSize)
        {
            // Start with a base query to retrieve users that have not been soft deleted
            IQueryable<User> query = _unitOfWork.GetRepository<User>().Entities.Where(user => !user.IsDeleted);

            // Apply filters based on search criteria
            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(user => user.Username.ToLower().Contains(username.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(user => user.FullName.ToLower().Contains(fullName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(user => user.Role.ToLower().Contains(role.ToLower()));
            }

            // Retrieve the filtered list of users
            IEnumerable<User> retrieveUsers = await query.ToListAsync();

            // Get the total number of users that match the criteria
            int totalUsers = await query.CountAsync();

            // Verify if the total number of users is zero
            if (totalUsers == 0)
            {
                throw new ArgumentException("No users found");
            }

            // 
            IReadOnlyCollection<User> paginatedUsers = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Map the User entities to UserResponseModel
            IReadOnlyCollection<UserSummaryModel> userModel = _mapper.Map<IReadOnlyCollection<User>, IReadOnlyCollection<UserSummaryModel>>(paginatedUsers);

            // Create the paginated list
            BasePaginatedList<UserSummaryModel> paginatedList = new(userModel, totalUsers, pageNumber, pageSize);

            return paginatedList;
        }

        public async Task UpdatePasswordAsync(int id, string newPassword, string newConfirmPassword)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(id);

            // Verify if the new password or the confirmation password is null, empty, or consists only of whitespace
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(newConfirmPassword))
            {
                throw new ArgumentException("Password and confirm password cannot be empty or whitespace.");
            }

            // Verify if the new password and the confirmation password do not match
            if (newPassword != newConfirmPassword)
            {
                throw new ArgumentException("New password and confirm password do not match");
            }

            // Retrieve user by ID from the repository
            User existingUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Id == id && !user.IsDeleted) ?? throw new ArgumentException($"User with ID {id} not found");

            // Hash Password
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Update value
            existingUser.Password = newPasswordHash;
            existingUser.LastUpdatedTime = TimeHelper.GetUtcPlus7Time();

            // Update entity
            await _unitOfWork.GetRepository<User>().UpdateAsync(existingUser);
            await _unitOfWork.SaveAsync();

            // Need a function to check how many rows have been updated
        }

        public async Task UpdateFullNameAsync(int id, string newFullName)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(id);

            // Verify if the new full name is null, empty, or consists only of whitespace
            if (string.IsNullOrWhiteSpace(newFullName))
            {
                throw new ArgumentException("New full name cannot be empty or whitespace");
            }

            // Retrieve user by ID from the repository
            User existingUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Id == id && !user.IsDeleted) ?? throw new ArgumentException($"User with ID {id} not found");

            // Update value
            existingUser.FullName = newFullName.Trim();
            existingUser.LastUpdatedTime = TimeHelper.GetUtcPlus7Time();

            // Update entity
            await _unitOfWork.GetRepository<User>().UpdateAsync(existingUser);
            await _unitOfWork.SaveAsync();

            // Need a function to check how many rows have been updated
        }

        public async Task DeleteUserByIDAsync(int id)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(id);

            // Retrieve user by ID from the repository
            User retrieveUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Id == id && (!user.IsDeleted || !user.DeletedTime.HasValue)) ?? throw new ArgumentException($"User with ID {id} not found or has been deleted");

            if (retrieveUser.Role == "Admin")
            {
                throw new ArgumentException("Can not delete a user with admin role");
            }

            // Check if there is still a User in AssemblyLine? If it exists, it cannot be deleted
            bool isExistingUserInAssemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().Entities.AnyAsync(al => al.ManagerId == retrieveUser.Id && !al.IsDeleted);
            if (isExistingUserInAssemblyLine)
            {
                throw new ArgumentException("There is still a manager in the assembly line");
            }

            // Soft Delete
            retrieveUser.DeletedTime = TimeHelper.GetUtcPlus7Time();
            retrieveUser.IsDeleted = true;

            // Update entity
            await _unitOfWork.GetRepository<User>().UpdateAsync(retrieveUser);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteUserByUsernameAsync(string username)
        {
            // Retrieve user by username from the repository
            User retrieveUser = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Username == username && (!user.IsDeleted || !user.DeletedTime.HasValue)) ?? throw new ArgumentException($"User with Username {username} not found or has been deleted");

            if (retrieveUser.Role == "Admin")
            {
                throw new ArgumentException("Can not delete a user with admin role");
            }

            // Check if there is still a User in AssemblyLine? If it exists, it cannot be deleted
            bool isExistingUserInAssemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().Entities.AnyAsync(al => al.ManagerId == retrieveUser.Id && !al.IsDeleted);
            if (isExistingUserInAssemblyLine)
            {
                throw new ArgumentException("There is still a manager in the assembly line");
            }

            // Soft Delete
            retrieveUser.DeletedTime = TimeHelper.GetUtcPlus7Time();
            retrieveUser.IsDeleted = true;

            // Update entity
            await _unitOfWork.GetRepository<User>().UpdateAsync(retrieveUser);
            await _unitOfWork.SaveAsync();
        }

        private void CheckInvalidID(int firstID, int? secondID = null, int? thirdID = null)
        {
            // If the ID is less than 1 throw ArgumentException
            if (firstID < 1 || (secondID.HasValue && secondID < 1) || (thirdID.HasValue && thirdID < 1))
            {
                throw new ArgumentException("Invalid ID");
            }

            return;
        }
    }
}
