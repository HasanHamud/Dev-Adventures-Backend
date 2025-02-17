using Dev_Db.Data;
using Dev_Models.DTOs.PlanDTO;
using Dev_Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dev_Adventures_Backend.Controllers.Plan
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        public readonly Dev_DbContext _context;

        public PlanController(Dev_DbContext context) {

            _context = context;
        
        }

        [HttpPost]
        public async Task<ActionResult> CreatePlan(CreatePlanDTO planDto)
        {
            try
            {
                // Validate course IDs exist
                var courses = await _context.Courses
                    .Where(c => planDto.CourseIds.Contains(c.Id))
                    .ToListAsync();

                if (courses.Count != planDto.CourseIds.Count)
                {
                    return BadRequest("One or more course IDs are invalid");
                }

                decimal totalPrice = courses.Sum(c => c.Price);

                decimal discountedPrice = totalPrice * (1 - (planDto.Discount / 100m));

                var plan = new Dev_Models.Models.Plan
                {
                    Title = planDto.Title,
                    Description = planDto.Description,
                    Discount = planDto.Discount,
                    Level = planDto.Level,
                    totalPrice = discountedPrice,
                    PlansCourses = new List<PlansCourses>()
                };

                // Add courses to plan with order index
                for (int i = 0; i < courses.Count; i++)
                {
                    plan.PlansCourses.Add(new PlansCourses
                    {
                        Course = courses[i],
                        OrderIndex = i,
                        DateAdded = DateTime.UtcNow
                    });
                }

                _context.Plans.Add(plan);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetPlan),
                    new { id = plan.Id },
                    plan
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("/{id}")]
        public async Task<ActionResult> GetPlan([FromRoute] int id)
        {
            var plan = await _context.Plans
                .Include(p => p.PlansCourses)
                    .ThenInclude(pc => pc.Course)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
            {
                return NotFound();
            }



            return Ok(plan);
        }


        [HttpPut]
        [Route("/{id}")]
        public async Task<ActionResult> UpdatePlan([FromRoute] int id, [FromBody] UpdatePlanDTO updatePlan)
        {
            try
            {
                var plan = await _context.Plans
                    .Include(p => p.PlansCourses)
                        .ThenInclude(pc => pc.Course)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (plan == null)
                {
                    return NotFound();
                }

                plan.Title = updatePlan.Title;
                plan.Description = updatePlan.Description;
                plan.Level = updatePlan.Level;
                plan.Discount = updatePlan.Discount;

                if (updatePlan.CourseIds != null && updatePlan.CourseIds.Any())
                {
                    var newCourses = await _context.Courses
                        .Where(c => updatePlan.CourseIds.Contains(c.Id))
                        .ToListAsync();

                    if (newCourses.Count != updatePlan.CourseIds.Count)
                    {
                        return BadRequest("One or more course IDs are invalid");
                    }

                    _context.PlansCourses.RemoveRange(plan.PlansCourses);

                    decimal totalPrice = newCourses.Sum(c => c.Price);
                    plan.totalPrice = totalPrice * (1 - (plan.Discount / 100m));

                    for (int i = 0; i < newCourses.Count; i++)
                    {
                        plan.PlansCourses.Add(new PlansCourses
                        {
                            Course = newCourses[i],
                            OrderIndex = i,
                            DateAdded = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("/{id}")]
        public async Task<IActionResult> DeletePlan([FromRoute] int id)
        {

            var plan = await _context.Plans
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
            {
                return NotFound();
            }

            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();

            return Ok(plan);

        }
       
        [HttpGet]
        public async Task<ActionResult> GetAllPlans()
        {
            try
            {
                var plans = await _context.Plans
                    .Include(p => p.PlansCourses)
                        .ThenInclude(pc => pc.Course)
                    .Select(p => new
                    {
                        id = p.Id,
                        title = p.Title,
                        description = p.Description,
                        discount = p.Discount,
                        level = p.Level,
                        totalPrice = p.totalPrice,
                        courses = p.PlansCourses
                            .OrderBy(pc => pc.OrderIndex)
                            .Select(pc => new
                            {
                                id = pc.Course.Id,
                                title = pc.Course.Title,
                                orderIndex = pc.OrderIndex
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return Ok(plans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }


}

