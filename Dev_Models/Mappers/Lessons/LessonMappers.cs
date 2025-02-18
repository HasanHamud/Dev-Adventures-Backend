using Dev_Models.DTOs.LessonDTO;
using Dev_Models.Models;

namespace Dev_Models.Mappers.Lessons
{
    public static class LessonMappers
    {

        public static LessonDTO ToLessonDto(this Lesson LessonModel)
        {
            return new LessonDTO
            {
                Id = LessonModel.Id,
                Title = LessonModel.Title,
                Description = LessonModel.Description,
            };
        }

        public static Lesson ToLessonFromCreateDTO(this CreateLessonRequestDTO LessonModel)
        {
            return new Lesson
            {
                Title = LessonModel.Title,
                Description = LessonModel.Description,
            };
        }

        public static void UpdateLessonFromDTO(this Lesson LessonModel, UpdateLessonRequestDTO updateDTO)
        {
            LessonModel.Title = updateDTO.Title;
            LessonModel.Description = updateDTO.Description;
        }
    }
}

