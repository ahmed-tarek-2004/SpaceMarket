using Microsoft.AspNetCore.Http;

namespace Ecommerce.DataAccess.Services.ImageUploading
{
    public interface IImageUploadService
    {
        Task<string> UploadAsync(IFormFile file);
        Task<string> UploadCertificateAsync(IFormFile file, string providerId);

    }
}
