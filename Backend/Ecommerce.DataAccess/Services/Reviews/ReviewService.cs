using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.Notifications;
using Ecommerce.DataAccess.Services.Reviews;
using Ecommerce.Entities.DTO.Reviews;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace Ecommerce.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ReviewService> _logger;
        private readonly ResponseHandler _responseHandler;
        private readonly INotificationService _notificationService;
        public ReviewService(ApplicationDbContext db, ILogger<ReviewService> logger, ResponseHandler responseHandler,INotificationService notificationService)
        {
            _db = db;
            _logger = logger;
            _responseHandler = responseHandler;
            _notificationService=notificationService;
        }

        public async Task<Response<ReviewResponseDto>> CreateAsync(CreateReviewRequest request, string clientId)
        {
            try
            {
                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    ServiceId = request.ServiceId,
                    ClientId =clientId,
                    ProviderId = request.ProviderId,
                    Rating = request.Rating,
                    Text = request.Text,
                    CreatedAt = DateTime.UtcNow
                };

                await _db.Reviews.AddAsync(review);
                await _db.SaveChangesAsync();


                await _notificationService.NotifyUserAsync(
                                           recipientId: clientId,
                                           senderId: null,
                                           title: "Review Updated",
                                           message: "Your Review Has Been Added"
                                        );

                var dto = await ProjectToDto(review.Id);
                return _responseHandler.Created(dto, "Review created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return _responseHandler.ServerError<ReviewResponseDto>(ex.Message);
            }
        }

        public async Task<Response<ReviewResponseDto>> UpdateAsync(UpdateReviewRequest request)
        {
            try
            {
                var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == request.Id);
                if (review == null)
                    return _responseHandler.NotFound<ReviewResponseDto>("Review not found");

                if (request.Rating.HasValue) review.Rating = request.Rating.Value;
                if (request.Text != null) review.Text = request.Text;
                review.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                await _notificationService.NotifyUserAsync(
                                           recipientId: review.ClientId,
                                           senderId: null,
                                           title: "Review Update",
                                           message: "Your Review Has Been Updated"
                                        );



                var dto = await ProjectToDto(review.Id);
                return _responseHandler.Success(dto, "Review updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review");
                return _responseHandler.ServerError<ReviewResponseDto>(ex.Message);
            }
        }

        public async Task<Response<bool>> DeleteAsync(Guid id, string clientId)
        {
            try
            {
                var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.ClientId == clientId && !r.IsHidden);
                if (review == null)
                    return _responseHandler.NotFound<bool>("Review not found");

                var response = await _db.ReviewResponses.FirstOrDefaultAsync(rr => rr.ReviewId == review.Id);
                if (response != null) _db.ReviewResponses.Remove(response);

                _db.Reviews.Remove(review);
                await _db.SaveChangesAsync();

                return _responseHandler.Deleted<bool>("Review deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review");
                return _responseHandler.ServerError<bool>(ex.Message);
            }
        }

        public async Task<Response<List<ReviewResponseDto>>> GetByServiceAsync(Guid serviceId)
        {
            var reviews = await _db.Reviews
                .Include(r => r.Client)
                .Include(r => r.Response)
                .Where(r => r.ServiceId == serviceId && !r.IsHidden)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    ServiceId = r.ServiceId,
                    ClientId = r.ClientId,
                    ClientName = r.Client.FullName,
                    Rating = r.Rating,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt,
                    ProviderReply = r.Response != null ? r.Response.Text : null
                }).ToListAsync();

            return _responseHandler.Success(reviews, "Fetched reviews successfully");
        }

        public async Task<Response<List<ReviewResponseDto>>> GetForProviderAsync(string providerId)
        {
            var reviews = await _db.Reviews
                .Include(r => r.Service)
                .Include(r => r.Client)
                .Include(r => r.Response)
                .Where(r => r.Service.ProviderId == providerId && !r.IsHidden)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    ServiceId = r.ServiceId,
                    ClientId = r.ClientId,
                    ClientName = r.Client.FullName,
                    Rating = r.Rating,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt,
                    ProviderReply = r.Response != null ? r.Response.Text : null
                }).ToListAsync();

            return _responseHandler.Success(reviews, "Fetched provider reviews");
        }

        public async Task<Response<List<ReviewResponseDto>>> GetAllFilteredAsync(ReviewFilterRequest filter)
        {
            var query = _db.Reviews
                .Where(r => !r.IsHidden)
                .Include(r => r.Service)
                .Include(r => r.Client)
                .Include(r => r.Response)
                .AsQueryable();

            if (filter.ServiceId.HasValue)
                query = query.Where(r => r.ServiceId == filter.ServiceId);

            if (!string.IsNullOrEmpty(filter.ProviderId))
                query = query.Where(r => r.Service.ProviderId == filter.ProviderId);

            if (filter.Rating > 0 && filter.Rating <= 5)
                query = query.Where(r => r.Rating == filter.Rating);

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    ServiceId = r.ServiceId,
                    ClientId = r.ClientId,
                    ClientName = r.Client.FullName,
                    Rating = r.Rating,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt,
                    ProviderReply = r.Response != null ? r.Response.Text : null
                }).ToListAsync();

            return _responseHandler.Success(reviews, "Fetched filtered reviews");
        }

        public async Task<Response<ReviewResponseDto>> ProviderReplyAsync(ProviderReplyRequest request, string providerId)
        {
            try
            {
                var review = await _db.Reviews
                    .Where(r=> !r.IsHidden)
                    .Include(r => r.Service)
                    .FirstOrDefaultAsync(r => r.Id == request.ReviewId && r.Service.ProviderId == providerId);

                if (review == null)
                    return _responseHandler.NotFound<ReviewResponseDto>("Review not found");

                var existingResponse = await _db.ReviewResponses
                    .FirstOrDefaultAsync(rr => rr.ReviewId == review.Id);

                if (existingResponse == null)
                {
                    var response = new ReviewResponse
                    {
                        Id = Guid.NewGuid(),
                        ReviewId = review.Id,
                        ProviderId = providerId,
                        Text = request.ReplyText,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _db.ReviewResponses.AddAsync(response);
                }
                else
                {
                    existingResponse.Text = request.ReplyText;
                    existingResponse.UpdatedAt = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();

                var dto = await ProjectToDto(review.Id);
                return _responseHandler.Success(dto, "Reply added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding provider reply");
                return _responseHandler.ServerError<ReviewResponseDto>(ex.Message);
            }
        }

        public async Task<Response<bool>> AdminHideOrDeleteAsync(Guid reviewId, bool delete)
        {
            try
            {
                var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
                if (review == null)
                    return _responseHandler.NotFound<bool>("Review not found");

                if (delete)
                {
                    var response = await _db.ReviewResponses.FirstOrDefaultAsync(rr => rr.ReviewId == review.Id);
                    if (response != null) _db.ReviewResponses.Remove(response);

                    _db.Reviews.Remove(review);
                }
                else
                {
                    review.IsHidden = true;
                    review.UpdatedAt = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();
                return _responseHandler.Success(true, delete ? "Review deleted" : "Review hidden");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hiding/deleting review");
                return _responseHandler.ServerError<bool>(ex.Message);
            }
        }


        // Helper: projection to DTO
        private async Task<ReviewResponseDto> ProjectToDto(Guid reviewId)
        {
            return await _db.Reviews
                .Include(r => r.Client)
                .Include(r => r.Response)
                .Where(r => r.Id == reviewId)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    ServiceId = r.ServiceId,
                    ClientId = r.ClientId,
                    ClientName = r.Client.FullName,
                    Rating = r.Rating,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt,
                    ProviderReply = r.Response != null ? r.Response.Text : null
                }).FirstOrDefaultAsync();
        }
    }
}
