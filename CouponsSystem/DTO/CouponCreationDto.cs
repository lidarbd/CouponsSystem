﻿namespace CouponsSystem.DTO
{
    public class CouponCreationDto
    {
        public required string Description { get; set; }
        public required string Code { get; set; }
        public int UserCreatorID { get; set; }
        public bool IsPercentage { get; set; }
        public double Discount { get; set; }
        public bool IsMultipleDiscounts { get; set; }
        public int? MaxUsageCount { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
