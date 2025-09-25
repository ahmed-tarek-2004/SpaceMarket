using Microsoft.AspNetCore.Http;

namespace Ecommerce.DataAccess.Services.ImageUploading
{
    public interface IImageUploadService
    {
        Task<string> UploadAsync(IFormFile file);
        Task<bool> DeleteAsync(string publicId);

        Task<string> UploadCertificateAsync(IFormFile file, string providerId);

    }
}
