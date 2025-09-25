using Ecommerce.Entities.DTO.ServiceCategory;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.DataAccess.Services.ServiceCategory
{
    public interface ICategoryService
    {
        Task<Response<Guid>> AddServiceCategoryAsync(CreateServiceCategoryRequest dto, CancellationToken cancellationToken = default);
        Task<Response<List<GetServiceCategoryResponse>>> GetAllServiceCategoriesAsync(CancellationToken cancellationToken = default);
        Task<Response<GetServiceCategoryResponse>> GetServiceCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Response<GetServiceCategoryResponse>> GetServiceCategoryByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Response<bool>> DeleteServiceCategoryAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Response<Guid>> UpdateServiceCategoryAsync(Guid id, UpdateServiceCategoryRequest dto, CancellationToken cancellationToken = default);

    }
}