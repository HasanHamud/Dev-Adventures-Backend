using Dev_Models.DTOs.Courses;
using Dev_Models.DTOs.LessonDTO;
using Dev_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Length = LessonModel.Length,
              
            };
        }

        public static Lesson ToLessonFromCreateDTO(this CreateLessonRequestDTO LessonModel)
        {
            return new Lesson
            {
                Title = LessonModel.Title,
                Description = LessonModel.Description,
                Length = LessonModel.Length,


            };
        }

        public static void UpdateLessonFromDTO(this Lesson LessonModel, UpdateLessonRequestDTO updateDTO)
        {
            LessonModel.Title = updateDTO.Title;
            LessonModel.Description = updateDTO.Description;
            LessonModel.Length = updateDTO.Length;
          
        }
    }




}

