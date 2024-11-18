namespace CouponsSystem.Models
{
    public class Coupons
    {
        public string Code { get; set; } = string.Empty;   // Primary Key
        public string Description { get; set; } = string.Empty;
        public int UserCreatorID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsPercentage { get; set; }
        public double Discount { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsMultipleDiscounts { get; set; }
        public int? MaxUsageCount { get; set; }
    }
}
