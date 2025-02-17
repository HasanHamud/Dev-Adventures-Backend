namespace Dev_Models.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string CouponCode { get; set; }
        public string? UserId { get; set; }
        public bool Used { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
