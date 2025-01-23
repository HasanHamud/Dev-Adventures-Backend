using Dev_Models.DTOs.Courses;
using Dev_Models.Models;

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
                Requirements = courseModel.Requirements?.Select(r => r.Description).ToList() ?? new List<string>(),
                LearningObjectives = courseModel.LearningObjectives?.Select(o => o.Description).ToList() ?? new List<string>()
            };
        }

        public static Course ToCourseFromCreateDTO(this CreateCourseRequestDTO courseDTO)
        {
            var course = new Course
            {
                Title = courseDTO.Title,
                Description = courseDTO.Description,
                Rating = courseDTO.Rating,
                Price = courseDTO.Price,
                ImgURL = courseDTO.ImgURL,
                Level = Enum.Parse<Course.CourseLevel>(courseDTO.Level, true),
                Duration = courseDTO.Duration,
                Language = courseDTO.Language,
                Status = Enum.Parse<Course.CourseStatus>(courseDTO.Status, true),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            return course;
        }

        public static void UpdateCourseFromDTO(this Course courseModel, UpdateCourseRequestDTO updateDTO)
        {
            courseModel.Title = updateDTO.Title;
            courseModel.Description = updateDTO.Description;
            courseModel.Rating = updateDTO.Rating;
            courseModel.Price = updateDTO.Price;
            courseModel.ImgURL = updateDTO.ImgURL;
            courseModel.Level = Enum.Parse<Course.CourseLevel>(updateDTO.Level, true);
            courseModel.Duration = updateDTO.Duration;
            courseModel.Language = updateDTO.Language;
            courseModel.Status = Enum.Parse<Course.CourseStatus>(updateDTO.Status, true);
            courseModel.UpdatedDate = DateTime.Now;


        }
    }
}