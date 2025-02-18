using Microsoft.AspNetCore.Http;

namespace Dev_Models.Mappers.Profile
{
    public class ProfileMappers
    {

        public static async Task<string?> SaveProfileImageAsync(IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type. Only .jpg, .jpeg, .png, and .gif files are allowed.");

            const int maxFileSize = 5 * 1024 * 1024;
            if (profileImage.Length > maxFileSize)
                throw new InvalidOperationException("File size exceeds the 5MB limit.");

            var uploadDirectory = Path.Combine("wwwroot", "images", "profiles");
            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadDirectory, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            return $"/images/profiles/{fileName}";
        }
    }
}