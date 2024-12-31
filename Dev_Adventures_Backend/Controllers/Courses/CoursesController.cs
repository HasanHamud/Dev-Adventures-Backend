using Dev_Db.Data;
using Dev_Models.DTOs.Courses;
using Dev_Models.Mappers.Courses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dev_Adventures_Backend.Controllers.Courses
{

    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly Dev_DbContext _context;
        public CoursesController(Dev_DbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var courses = await _context.Courses.ToListAsync();
            
            var courseDto = courses.Select(c => c.toCourseDto());
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }
      

        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CreateCourseRequestDTO coursedto)
        {
            var courseModel = coursedto.toCourseFromCreateDTO();
            await _context.Courses.AddAsync(courseModel);
            await  _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new {id = courseModel.Id},courseModel.toCourseDto());

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] int id , [FromBody] UpdateCourseRequestDTO updateDTO)
        {
            var courseModel = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);


            if (courseModel == null)
            {
                return NotFound();
            }
            courseModel.Title = updateDTO.Title;
            courseModel.Description = updateDTO.Description;
            courseModel.ImgURL = updateDTO.ImgURL;
            courseModel.Rating = updateDTO.Rating;
            courseModel.Price = updateDTO.Price;
            await _context.SaveChangesAsync();

            return Ok(courseModel);



        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] int id)
        {
            var stockModel = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);

            if (stockModel == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(stockModel);

            await _context.SaveChangesAsync();

            return NoContent();
        }







    }
}
