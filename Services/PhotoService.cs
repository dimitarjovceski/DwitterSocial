using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DwitterSocial.Helpers;
using DwitterSocial.Interfaces;
using Microsoft.Extensions.Options;

namespace DwitterSocial.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);

            cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadRes = new ImageUploadResult();

            if(file.Length > 0)
            {
               using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                };

                uploadRes = await cloudinary.UploadAsync(uploadParams);
            }
            return uploadRes;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}
