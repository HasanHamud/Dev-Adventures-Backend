using Dev_Models.DTOs.Courses;
using Dev_Models.Models;


namespace Dev_Models.Mappers.Courses
{
    public static class CourseMappers
    {
        public static CourseDTO toCourseDto(this Course courseModel)
        {
            return new CourseDTO
            {
                Id = courseModel.Id,
                Title = courseModel.Title,
                ImgURL = courseModel.ImgURL,
                Rating
                = courseModel.Rating,
                Description = courseModel.Description,
                Price = courseModel.Price
            };

        }

        public static Course toCourseFromCreateDTO(this CreateCourseRequestDTO courseDTO)
        {

            return new Course
            {
                Title = courseDTO.Title,
                ImgURL = courseDTO.ImgURL,
                Rating = courseDTO.Rating,
                Description = courseDTO.Description,
                Price = courseDTO.Price
            };
        }

    }
}
