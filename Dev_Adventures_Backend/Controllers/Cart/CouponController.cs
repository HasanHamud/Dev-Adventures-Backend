using Dev_Db.Data;
using Dev_Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class CouponController : ControllerBase
{
    private readonly Dev_DbContext _context;

    public CouponController(Dev_DbContext context)
    {
        _context = context;
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GenerateCoupon()
    {
        var couponCode = Guid.NewGuid().ToString().Substring(0, 8);

        var coupon = new Coupon
        {
            CouponCode = couponCode,
            UserId = null,
            Used = false,
            ExpirationDate = DateTime.UtcNow.AddDays(5)
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        return Ok(new { CouponCode = couponCode });
    }


    [HttpPost("apply")]
    [Authorize]
    public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized("User not authenticated");

        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.CouponCode == request.CouponCode);

        if (coupon == null)
            return BadRequest("Invalid coupon.");

        if (coupon.Used)
            return BadRequest("Coupon already used.");

        if (coupon.ExpirationDate < DateTime.UtcNow)
            return BadRequest("Coupon expired.");

        var userCart = await _context.Carts
            .Include(c => c.courses)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (userCart == null)
            return NotFound("Cart not found");

        var course = userCart.courses.FirstOrDefault(c => c.Id == request.CourseId);
        if (course == null)
            return NotFound("Course not found in the cart");

        userCart.totalPrice -= course.Price;

        coupon.Used = true;

        await _context.SaveChangesAsync();

        return Ok("Coupon applied successfully. The selected course is now free!");
    }

}
