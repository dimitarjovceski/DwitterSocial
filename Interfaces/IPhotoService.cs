using CloudinaryDotNet.Actions;

namespace DwitterSocial.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string photoId);
    }
}
