using Dev_Models.DTOs.Courses;
using Dev_Models.Models;
using Microsoft.AspNetCore.Http;

namespace Dev_Models.Mappers.Courses
{
    public static class CourseMappers
    {
        public static CourseDTO ToCourseDto(this Course courseModel)
        {
            return new CourseDTO
            {
                Id = courseModel.Id,
                Title = courseModel.Title,
                Description = courseModel.Description,
                Rating = courseModel.Rating,
                Price = courseModel.Price,
                ImgURL = courseModel.ImgURL,
                Level = courseModel.Level.ToString(),
                Duration = courseModel.Duration,
                Language = courseModel.Language,
                Status = courseModel.Status.ToString(),
                CreatedDate = courseModel.CreatedDate,
                UpdatedDate = courseModel.UpdatedDate,
                previewURL = courseModel.previewURL,  // Added previewURL mapping
                Requirements = courseModel.Requirements?.Select(r => r.Description).ToList() ?? new List<string>(),
                LearningObjectives = courseModel.LearningObjectives?.Select(o => o.Description).ToList() ?? new List<string>()
            };
        }

        public static async Task<Course> ToCourseFromCreateDTO(this CreateCourseRequestDTO courseDTO)
        {
            var imgURL = await SaveImageAsync(courseDTO.ImgURL);
            var course = new Course
            {
                Title = courseDTO.Title,
                Description = courseDTO.Description,
                Rating = courseDTO.Rating,
                Price = courseDTO.Price,
                ImgURL = imgURL,
                Level = Enum.Parse<Course.CourseLevel>(courseDTO.Level, true),
                Duration = courseDTO.Duration,
                Language = courseDTO.Language,
                Status = Enum.Parse<Course.CourseStatus>(courseDTO.Status, true),
                previewURL = courseDTO.previewURL,  // Added previewURL mapping
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            return course;
        }

        public static async Task UpdateCourseFromDTO(this Course courseModel, UpdateCourseRequestDTO updateDTO)
        {
            if (updateDTO.ImgURL != null)
            {
                courseModel.ImgURL = await SaveImageAsync(updateDTO.ImgURL);
            }
            courseModel.Title = updateDTO.Title;
            courseModel.Description = updateDTO.Description;
            courseModel.Rating = updateDTO.Rating;
            courseModel.Price = updateDTO.Price;
            courseModel.Level = Enum.Parse<Course.CourseLevel>(updateDTO.Level, true);
            courseModel.Duration = updateDTO.Duration;
            courseModel.Language = updateDTO.Language;
            courseModel.Status = Enum.Parse<Course.CourseStatus>(updateDTO.Status, true);
            courseModel.previewURL = updateDTO.previewURL;  // Added previewURL mapping
            courseModel.UpdatedDate = DateTime.Now;
        }

        private static async Task<string?> SaveImageAsync(IFormFile ImageURL)
        {
            if (ImageURL == null || ImageURL.Length == 0)
                return null;

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageURL.FileName)}";
            var filePath = Path.Combine("wwwroot/images/courses", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageURL.CopyToAsync(stream);
            }

            return $"/images/courses/{fileName}";
        }
    }
}