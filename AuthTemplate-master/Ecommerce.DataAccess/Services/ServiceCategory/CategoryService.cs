using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.Notifications;
using Ecommerce.Entities.DTO.ServiceCategory;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Ecommerce.DataAccess.Services.ServiceCategory
{
    public class CategoryService(ApplicationDbContext context,
     ILogger<CategoryService> logger,
     ResponseHandler responseHandler) : ICategoryService

    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<CategoryService> _logger = logger;
        private readonly ResponseHandler _responseHandler = responseHandler;

        public async Task<Response<Guid>> AddServiceCategoryAsync(CreateServiceCategoryRequest dto, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var isExist = await _context.ServiceCategories
                    .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && !c.IsDeleted, cancellationToken);
                if (isExist)
                {
                    _logger.LogWarning("Category with name {Name} already exists.", dto.Name);
                    return _responseHandler.BadRequest<Guid>("Category with the same name already exists.");
                }

                var category = new Entities.Models.ServiceCategory
                {
                    Name = dto.Name,
                    Description = dto.Description
                };
                await _context.ServiceCategories.AddAsync(category, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Category {CategoryId} added successfully.", category.Id);
                return responseHandler.Created(category.Id, "Category added successfully.");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("Failed to add category.");
                return _responseHandler.InternalServerError<Guid>("Failed to add category.");
            }


        }


        public async Task<Response<List<GetServiceCategoryResponse>>> GetAllServiceCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _context.ServiceCategories
                .Where(c => !c.IsDeleted)
                .Select(c => new GetServiceCategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return _responseHandler.Success(categories, "Categories retrieved successfully.");
        }

        public async Task<Response<GetServiceCategoryResponse>> GetServiceCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {

            var category = await GetServiceCategoryAsync(c => c.Id == id);
            if (category is null)
            {
                _logger.LogWarning("Category with ID {Id} not found.", id);
                return _responseHandler.NotFound<GetServiceCategoryResponse>("Category not found.");
            }

            return _responseHandler.Success(category, "Category retrieved successfully.");
        }

        public async Task<Response<GetServiceCategoryResponse>> GetServiceCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("GetCategoryByNameAsync called with empty name.");
                return _responseHandler.BadRequest<GetServiceCategoryResponse>("Invalid category name.");
            }

            var category = await GetServiceCategoryAsync(c => c.Name.ToLower().Contains(name.ToLower()));
            if (category is null)
            {
                _logger.LogWarning("Category with name {Name} not found.", name);
                return _responseHandler.NotFound<GetServiceCategoryResponse>("Category not found.");
            }

            return _responseHandler.Success(category, "Category retrieved successfully.");
        }


        public async Task<Response<Guid>> UpdateServiceCategoryAsync(Guid id, UpdateServiceCategoryRequest dto, CancellationToken cancellationToken = default)
        {

            var category = await _context.ServiceCategories.FindAsync(id, cancellationToken);
            if (category == null || category.IsDeleted)
            {
                _logger.LogWarning("UpdateCategoryAsync - Category not found. ID: {Id}", id);
                return _responseHandler.NotFound<Guid>("Category not found.");
            }

            if (category.Name == dto.Name && category.Description == dto.Description)
            {
                return _responseHandler.BadRequest<Guid>("No changes detected.");
            }

            // Check for duplication with another category (excluding current one)
            var existingCategory = await _context.ServiceCategories
                .FirstOrDefaultAsync(c =>
                    c.Id != id &&
                    c.Name == dto.Name &&
                    c.Description == dto.Description &&
                    !c.IsDeleted, cancellationToken);

            if (existingCategory != null)
            {
                return _responseHandler.BadRequest<Guid>("Another category with the same name and description already exists.");
            }
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.UpdatedAt = DateTime.UtcNow;

            _context.ServiceCategories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Category with ID {Id} updated successfully.", id);

            return _responseHandler.Success(category.Id, "Category updated successfully.");
        }

        public async Task<Response<bool>> DeleteServiceCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        {

            var category = await _context.ServiceCategories.FindAsync(id, cancellationToken);
            if (category == null || category.IsDeleted)
            {
                _logger.LogWarning("DeleteCategoryAsync - Category not found or already deleted. ID: {Id}", id);
                return _responseHandler.NotFound<bool>("Category not found or already deleted.");
            }

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;

            _logger.LogWarning("Category with ID {Id} is being soft deleted.", id);

            _context.ServiceCategories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Category with ID {Id} was deleted successfully.", id);

            return _responseHandler.Success(true, "Category deleted successfully.");
        }


        private async Task<GetServiceCategoryResponse?> GetServiceCategoryAsync(Expression<Func<Ecommerce.Entities.Models.ServiceCategory, bool>> predicate)
        {
            return await _context.ServiceCategories
                .Where(predicate)
                .Where(c => !c.IsDeleted)
                .Select(c => new GetServiceCategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();
        }
    }
}
