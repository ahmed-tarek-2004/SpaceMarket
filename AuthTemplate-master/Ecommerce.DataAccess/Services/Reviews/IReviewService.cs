using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.DTO.Reviews;
using Ecommerce.Entities.Shared.Bases;
namespace Ecommerce.DataAccess.Services.Reviews
{
    public interface IReviewService
    {
        Task<Response<ReviewResponseDto>> CreateAsync(CreateReviewRequest request, string clientId);
        Task<Response<ReviewResponseDto>> UpdateAsync(UpdateReviewRequest request);
        Task<Response<bool>> DeleteAsync(Guid id, string clientId);
        Task<Response<List<ReviewResponseDto>>> GetByServiceAsync(Guid serviceId);
        Task<Response<List<ReviewResponseDto>>> GetForProviderAsync(string providerId);
        Task<Response<List<ReviewResponseDto>>> GetAllFilteredAsync(ReviewFilterRequest filter);
        Task<Response<ReviewResponseDto>> ProviderReplyAsync(ProviderReplyRequest request, string providerId);
        Task<Response<bool>> AdminHideOrDeleteAsync(Guid reviewId, bool delete);
    }
}
