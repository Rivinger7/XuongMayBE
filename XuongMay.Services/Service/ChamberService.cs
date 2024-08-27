using AutoMapper;
using AutoMapper.QueryableExtensions;
using GarmentFactory.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.ModelViews.InventoryHistoriesModelViews;

namespace XuongMay.Services.Service
{
	public class ChamberService : IChamberService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ChamberService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<BasePaginatedList<ResponseInventoryHistoryModel>> GetInventoryHistoriesAsync(int pageNumber, int pageSize, int chamberID, int? searchId, string? searchProductName, bool? importAndExport)
		{
			//Check chamber có tồn tại không
			ChamberProducts? chamber = await _unitOfWork.GetRepository<ChamberProducts>().GetByIdAsync(chamberID)
				?? throw new Exception("Khoang không tồn tại");

			//Tìm các InventoryChamberMapper có cùng chamberID
			IQueryable<InventoryChamberMappers> queryMapper = _unitOfWork.GetRepository<InventoryChamberMappers>()
				.Entities
				.Where(i => i.ChamberId == chamberID && !i.DeletedTime.HasValue);

			//Lọc theo ImportAndExport
			if (importAndExport.HasValue)
			{
				queryMapper = queryMapper.Where(i => i.InventoryHistories.IsImport == importAndExport.HasValue);
			}

			var listMapper = await queryMapper.Select(i => i.InventoryId).ToListAsync();

			//Tạo truy vấn lịch sử nhập/xuất hàng và Sắp xếp theo giảm dần về thời gian tạo
			IQueryable<InventoryHistories> query = _unitOfWork.GetRepository<InventoryHistories>()
				.Entities
				.Where(i => listMapper.Contains(i.Id) && !i.DeletedTime.HasValue)
				.OrderByDescending(i => i.CreatedTime);

			//Lọc theo ImportAndExport
			if (importAndExport.HasValue)
			{
				query = query.Where(i => i.IsImport == importAndExport.HasValue);
			}

			//lọc theo searchId
			if (searchId > 0)
			{
				query = query.Where(i => i.Id == searchId);
			}

			//Tìm kiếm theo ProductName nếu có giá trị
			if(!string.IsNullOrWhiteSpace(searchProductName))
			{
				query = query.Where(i => i.Product.Name.Contains(searchProductName));
			}

			//Check page Size không được < 0
			if (pageSize < 0)
			{
				throw new Exception("Kích thước trang phải lớn hơn 0");
			}

			// Đếm tổng số lượng sau khi đã lọc
			int totalCategory = await query.CountAsync();

			//Áp dụng phân trang
			List<ResponseInventoryHistoryModel> pageInventoryHistory = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<ResponseInventoryHistoryModel>(_mapper.ConfigurationProvider)
				.ToListAsync();

			//Tạo BasePaginatedList và trả về
			return new BasePaginatedList<ResponseInventoryHistoryModel>(pageInventoryHistory, totalCategory, pageNumber, pageSize);
		}
	}
}
