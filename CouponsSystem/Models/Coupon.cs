using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CouponsSystem.Models
{
    public class Coupon
    {
        [Key]
        public string Code { get; set; } = string.Empty;   
        public string Description { get; set; } = string.Empty;
        [ForeignKey("UserCreatorID")]
        public int UserCreatorID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsPercentage { get; set; }
        public double Discount { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsMultipleDiscounts { get; set; }
        public int? MaxUsageCount { get; set; }
    }
}
