using GarmentFactory.Repository.Context;
using GarmentFactory.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using XuongMay.Core.Utils;

namespace XuongMay.Repositories.UOW
{
    public class UserRepository
    {
        private readonly GarmentFactoryDBContext _dbContext;

        public UserRepository(GarmentFactoryDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Only get users that have not been soft deleted
            IEnumerable<User> users = await _dbContext.Users
                .Where(user => !user.IsDeleted)
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<User>> GetAllAdminsAsync()
        {
            // Only get users that have not been soft deleted
            IEnumerable<User> users = await _dbContext.Users
                .Where(user => user.Role == "Admin" && !user.IsDeleted)
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<User>> GetAllManagersAsync()
        {
            // Only get users that have not been soft deleted
            IEnumerable<User> users = await _dbContext.Users
                .Where(user => user.Role == "Manager" && !user.IsDeleted)
                .ToListAsync();

            return users;
        }

        public async Task<User> GetByIDAsync(int id)
        {
            // Tìm kiếm người dùng theo ID và chưa bị xóa mềm
            User retrieveUser = await _dbContext.Users
                .FirstOrDefaultAsync(user => user.Id == id && !user.IsDeleted);

            if (retrieveUser is null)
            {
                throw new ArgumentException("The Account does not exist", "accountNotFound");
            }

            return retrieveUser;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {

            User retrieveUser = await _dbContext.Users
                .FirstOrDefaultAsync(user => user.Username == username && !user.IsDeleted);

            if (retrieveUser is null)
            {
                throw new ArgumentException("The Account does not exist", "accountNotFound");
            }

            return retrieveUser;
        }

        public async Task<User> GetByUsernameForRegister(string username)
        {
            User retrieveUser = await _dbContext.Users
                .FirstOrDefaultAsync(user => user.Username == username && !user.IsDeleted); // It should be Any()

            if (retrieveUser is not null)
            {
                throw new ArgumentException("The Account exists", "accountExisted");
            }

            return retrieveUser;
        }

        public async Task<IEnumerable<User>> GetByFullNameAsync(string fullName)
        {
            IEnumerable<User> users = await _dbContext.Users
                .Where(user => user.FullName.Contains(fullName) && !user.IsDeleted)
                .ToListAsync();
            if (!users.Any())
            {
                throw new ArgumentException("Not found!", "notFound");
            }

            return users;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(string? username, string? fullName, string? role)
        {
            try
            {
                var query = _dbContext.Users.AsQueryable();

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

                var users = await query.ToListAsync();

                if (!users.Any())
                {
                    throw new ArgumentException("No users found matching the given criteria.", "notFound");
                }

                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }


        public async Task CreateAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return;
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentException("User cannot be null", "isUserNull");
                }

                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(user => user.Id == user.Id && !user.IsDeleted);

                if (existingUser is null)
                {
                    throw new ArgumentException("The Account does not exist", "accountNotFound");
                }

                existingUser.FullName = user.FullName;
                existingUser.Password = user.Password;
                existingUser.LastUpdatedTime = TimeHelper.GetUtcPlus7Time();

                int rowsAffected = await _dbContext.SaveChangesAsync();

                if (rowsAffected == 0)
                {
                    throw new ArgumentException("No rows were updated. The update operation may have failed.", "updateFail");
                }

                return;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentException("User cannot be null", "isUserNull");
                }

                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == user.Id && !u.IsDeleted);

                if (existingUser is null)
                {
                    throw new ArgumentException("The Account does not exist", "accountNotFound");
                }

                // Perform a soft delete by updating the IsDeleted flag and DeletedTime
                existingUser.IsDeleted = true;
                existingUser.DeletedTime = TimeHelper.GetUtcPlus7Time(); // Change to UTC+7 later if needed

                await _dbContext.SaveChangesAsync();

                return;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
