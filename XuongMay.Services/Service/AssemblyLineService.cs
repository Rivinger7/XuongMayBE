using AutoMapper;
using GarmentFactory.Repository.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.AssemblyLineModelView;
using XuongMay.ModelViews.AssemblyLineModelViews;

namespace XuongMay.Services.Service
{
    public class AssemblyLineService : IAssemblyLineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssemblyLineService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BasePaginatedList<AssemblyLineModelView>> GetAllAssemblyLineAsync(int pageNumber, int pageSize)
        {
            // Retrieve all assembly lines from the repository
            IEnumerable<AssemblyLine> retrieveAssemblyLines = await _unitOfWork.GetRepository<AssemblyLine>().Entities.Where(al => !al.IsDeleted).ToListAsync();

            // Verify if the list of retrieved assembly line is empty
            if (!retrieveAssemblyLines.Any())
            {
                throw new ArgumentException("No assembly lines found");
            }

            // Get the total number of assembly lines
            int totalAssemblyLines = retrieveAssemblyLines.Count();

            // Pagination
            IReadOnlyCollection<AssemblyLine> paginatedassemblyLine = retrieveAssemblyLines.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            IReadOnlyCollection<AssemblyLineModelView> assemblyLineModel = _mapper.Map<IReadOnlyCollection<AssemblyLine>, IReadOnlyCollection<AssemblyLineModelView>>(paginatedassemblyLine);

            // Create the paginated list
            BasePaginatedList<AssemblyLineModelView> paginatedList = new (assemblyLineModel, totalAssemblyLines, pageNumber, pageSize);

            return paginatedList;
        }

        public async Task<AssemblyLineModelView> GetAssemblyLineByIDAsync(int id)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(id);

            // Retrieve assembly line by ID from the repository
            AssemblyLine retrieveAssemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().Entities.FirstOrDefaultAsync(al => al.Id == id && !al.IsDeleted) ?? throw new ArgumentException($"Assembly line with ID {id} not found");

            // Map the Assembly line entities to AssemblyLineModelView
            AssemblyLineModelView assemblyLineModel = _mapper.Map<AssemblyLine, AssemblyLineModelView>(retrieveAssemblyLine);

            return assemblyLineModel;
        }

        public async Task<AssemblyLineModelView> GetAssemblyLineByManagerIDAsync(int managerID)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(managerID);

            // Retrieve assembly line form the repository
            AssemblyLine retrieveAssemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().Entities.FirstOrDefaultAsync(manager => manager.ManagerId == managerID) ?? throw new ArgumentException($"Assembly line with Manager ID {managerID} not found");

            // Map the Assembly line entities to AssemblyLineModelView
            AssemblyLineModelView assemblyLineModel = _mapper.Map<AssemblyLine, AssemblyLineModelView>(retrieveAssemblyLine);

            return assemblyLineModel;
        }

        public async Task<BasePaginatedList<AssemblyLineModelView>> GetAssemblyLinesByFilteringAsync(string? assemblyLineName, string? description, int pageNumber, int pageSize)
        {
            // Start with a base query to retrieve assembly line that have not been soft deleted
            IQueryable<AssemblyLine> query = _unitOfWork.GetRepository<AssemblyLine>().Entities.Where(al => !al.IsDeleted);

            // Apply filters based on search criteria
            if (!string.IsNullOrWhiteSpace(assemblyLineName))
            {
                query = query.Where(user => user.Name.ToLower().Contains(assemblyLineName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(user => user.Description.ToLower().Contains(description.ToLower()));
            }

            // Retrieve the filtered list of assembly line
            IEnumerable<AssemblyLine> retrieveAssemblyLines = await query.ToListAsync();

            // Get the total number of assembly lines that match the criteria
            int totalAssemblyLines = await query.CountAsync();

            // Verify if the total number of assembly lines is zero
            if (totalAssemblyLines == 0)
            {
                throw new ArgumentException("No assembly lines found");
            }

            // Pagination
            IReadOnlyCollection<AssemblyLine> paginatedAssemblyLine = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Map the User entities to UserResponseModel
            IReadOnlyCollection<AssemblyLineModelView> assemblyLineModel = _mapper.Map<IReadOnlyCollection<AssemblyLine>, IReadOnlyCollection<AssemblyLineModelView>>(paginatedAssemblyLine);

            // Create the paginated list
            BasePaginatedList<AssemblyLineModelView> paginatedList = new(assemblyLineModel, totalAssemblyLines, pageNumber, pageSize);

            return paginatedList;
        }

        public async Task<BasePaginatedList<AssemblyLineModelView>> GetAssemblyLinesByCreatorAsync(string creator, int pageNumber, int pageSize)
        {
            // Retrieve all assembly lines from the repository
            IEnumerable<AssemblyLine> retrieveAssemblyLines = await _unitOfWork.GetRepository<AssemblyLine>().Entities.Where(al => !al.IsDeleted && al.CreatedBy.ToLower().Contains(creator.ToLower())).ToListAsync();
            if (!retrieveAssemblyLines.Any())
            {
                throw new ArgumentException($"List of assembly line with Creator full name {creator} not found");
            }

            // Get the total number of assembly lines
            int totalAssemblyLines = retrieveAssemblyLines.Count();

            // Pagination
            IReadOnlyCollection<AssemblyLine> paginatedassemblyLine = retrieveAssemblyLines.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            IReadOnlyCollection<AssemblyLineModelView> assemblyLineModel = _mapper.Map<IReadOnlyCollection<AssemblyLine>, IReadOnlyCollection<AssemblyLineModelView>>(paginatedassemblyLine);

            // Create the paginated list
            BasePaginatedList<AssemblyLineModelView> paginatedList = new(assemblyLineModel, totalAssemblyLines, pageNumber, pageSize);

            return paginatedList;
        }

        public async Task CreateAssemblyLineAsync(AssemblyLineCreateModel assemblyLineModel)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(assemblyLineModel.ManagerID);

            // Assign value
            string assemblyLineName = assemblyLineModel.Name;

            // Verify if the assembly line name is null, empty, or consists only of whitespace
            if (string.IsNullOrWhiteSpace(assemblyLineName))
            {
                throw new ArgumentException("Assembly Line name cannot be empty or whitespace");
            }

            // Verify if the number of staffs is less than one
            if (assemblyLineModel.NumberOfStaffs < 1)
            {
                throw new ArgumentException("The number of staffs must be greater than 0");
            }

            // Check if the assembly line name exists in the database or not
            bool isExistingAssemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().Entities.AnyAsync(al => al.Name.ToLower() == assemblyLineName.ToLower().Trim() && !al.IsDeleted);
            if (isExistingAssemblyLine)
            {
                throw new ArgumentException("Assembly Line name is already exists");
            }

            // Check if the user with the specified ID is a Manager
            bool isManager = await _unitOfWork.GetRepository<User>().Entities.AnyAsync(manager => manager.Id == assemblyLineModel.ManagerID && manager.Role == "Manager");
            if (!isManager)
            {
                throw new ArgumentException("The assigned user is not manager");
            }

            // Check if ManagerID has been assigned to another AssemblyLine
            bool isAssignedManager = await _unitOfWork.GetRepository<AssemblyLine>().Entities.AnyAsync(al => al.ManagerId == assemblyLineModel.ManagerID && !al.IsDeleted);
            if (isAssignedManager)
            {
                throw new ArgumentException("The manager ID is already assigned in the other assembly line");
            }

            // If the system has more than one admin then use the solution below
            // Get User ID from Session (Admin)
            //string userID = _httpContextAccessor.HttpContext.Session.GetString("UserID");
            //if (string.IsNullOrWhiteSpace(userID))
            //{
            //    throw new ArgumentException("User ID is null");
            //}

            // Parse and Assign value
            //int adminID = Int32.Parse(userID); // Should add try catch here

            // Retrieve admin from the repository
            //User admin = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Id == adminID) ?? throw new ArgumentException("User not found");

            // Retrieve admin by role from the repository
            User onlyOneAdmin = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(admin => admin.Role == "Admin" && !admin.IsDeleted) ?? throw new ArgumentException("There is no admin to create an assembly line");

            // Add a new assembly line
            AssemblyLine newAssemblyLine = new()
            {
                Name = assemblyLineName,
                Description = assemblyLineModel.Description,
                NumberOfStaffs = assemblyLineModel.NumberOfStaffs,
                ManagerId = assemblyLineModel.ManagerID,
                CreatedBy = onlyOneAdmin.FullName,
                CreatedTime = TimeHelper.GetUtcPlus7Time()
            };

            // Insert to the repository
            await _unitOfWork.GetRepository<AssemblyLine>().InsertAsync(newAssemblyLine);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAssemblyLineAsync(int id, AssemblyLineUpdateModel assemblyLineModel)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(id, assemblyLineModel.ManagerID);

            // Retrieve assembly line by ID from the repository
            AssemblyLine existingAssemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().Entities.FirstOrDefaultAsync(al => al.Id == id && !al.IsDeleted) ?? throw new ArgumentException($"Assembly line with ID {id} not found");

            // Verify if the assembly line name is null, empty, or consists only of whitespace
            if (string.IsNullOrWhiteSpace(assemblyLineModel.Name))
            {
                throw new ArgumentException("Assembly Line name cannot be empty or whitespace");
            }

            // Verify if the number of staffs is less than one
            if (assemblyLineModel.NumberOfStaffs < 1)
            {
                throw new ArgumentException("The number of staffs must be greater than 0");
            }

            // Check if the username exists in the database or not
            bool isExistingAssemblyLineName = await _unitOfWork.GetRepository<AssemblyLine>().Entities.AnyAsync(al => al.Name.ToLower() == assemblyLineModel.Name.ToLower().Trim() && !al.IsDeleted && al.Id != existingAssemblyLine.Id);
            if (isExistingAssemblyLineName)
            {
                throw new ArgumentException("Assembly Line name is already exists");
            }

            // Check if the user with the specified ID is a Manager
            bool isManager = await _unitOfWork.GetRepository<User>().Entities.AnyAsync(manager => manager.Id == assemblyLineModel.ManagerID && manager.Role == "Manager" && !manager.IsDeleted);
            if (!isManager)
            {
                throw new ArgumentException("The assigned user is not manager");
            }

            // Check if ManagerID has been assigned to another AssemblyLine
            bool isAssignedManager = await _unitOfWork.GetRepository<AssemblyLine>().Entities.AnyAsync(al => al.ManagerId == assemblyLineModel.ManagerID && !al.IsDeleted && al.Id != id);
            if (isAssignedManager)
            {
                throw new ArgumentException("The manager ID is already assigned in the other assembly line");
            }

            // Update value
            existingAssemblyLine.Name = assemblyLineModel.Name.Trim();
            existingAssemblyLine.Description = assemblyLineModel.Description is not null ? assemblyLineModel.Description.Trim() : null;
            existingAssemblyLine.NumberOfStaffs = assemblyLineModel.NumberOfStaffs;
            existingAssemblyLine.LastUpdatedTime = TimeHelper.GetUtcPlus7Time();

            // Update
            await _unitOfWork.GetRepository<AssemblyLine>().UpdateAsync(existingAssemblyLine);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAssemblyLineByIDAsync(int id)
        {
            // Check ID if the ID is less than 1 throw ArgumentException
            CheckInvalidID(id);

            // Retrieve assembly line by ID from the repository
            AssemblyLine existingAssemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().Entities.FirstOrDefaultAsync(al => al.Id == id && !al.IsDeleted) ?? throw new ArgumentException($"Assembly line with ID {id} not found");

            // Check if there is still a AssemblyLine in Task? If it exists, it cannot be deleted
            bool isExistingAssemblyLineInTask = await _unitOfWork.GetRepository<Tasks>().Entities.AnyAsync(tasks => tasks.AssemblyLineId == id && !tasks.IsDeleted);
            if (isExistingAssemblyLineInTask)
            {
                throw new ArgumentException("There is still an assembly line in the task");
            }

            // Soft Delete
            existingAssemblyLine.DeletedTime = TimeHelper.GetUtcPlus7Time();
            existingAssemblyLine.IsDeleted = true;

            // Update
            await _unitOfWork.GetRepository<AssemblyLine>().UpdateAsync(existingAssemblyLine);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAssemblyLineByNameAsync(string assemblyLineName)
        {
            // Retrieve assembly line by name from the repository
            AssemblyLine existingAssemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().Entities.FirstOrDefaultAsync(al => al.Name.ToLower() == assemblyLineName.ToLower().Trim() && !al.IsDeleted) ?? throw new ArgumentException($"Assembly line with Name {assemblyLineName} not found");

            // Check if there is still a AssemblyLine in Task? If it exists, it cannot be deleted
            bool isExistingAssemblyLineInTask = await _unitOfWork.GetRepository<Tasks>().Entities.AnyAsync(tasks => tasks.AssemblyLineId == existingAssemblyLine.Id && !tasks.IsDeleted);
            if (isExistingAssemblyLineInTask)
            {
                throw new ArgumentException("There is still an assembly line in the task");
            }

            // Soft Delete
            existingAssemblyLine.DeletedTime = TimeHelper.GetUtcPlus7Time();
            existingAssemblyLine.IsDeleted = true;

            // Update
            await _unitOfWork.GetRepository<AssemblyLine>().UpdateAsync(existingAssemblyLine);
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
