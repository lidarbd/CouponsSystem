using CouponsSystem.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CouponsSystem.Models
{
    public class CouponsManager
    {
        private readonly AppDbContext _context;

        public CouponsManager(AppDbContext context)
        {
            _context = context;
        }

        // Insert a coupon into the database
        public async Task InsertAsync(string description, string code, int userCreatorID, DateTime createdDateTime, bool isPercentage, double discount, bool isMultipleDiscounts, int? maxUsageCount = null, DateTime? expirationDate = null)
        {
            // Check if the coupon code already exists in the system
            if (await _context.Coupons.AnyAsync(c => c.Code == code))
            {
                throw new InvalidOperationException("A coupon with this code already exists.");
            }

            var coupon = new Coupons
            {
                Code = code,
                Description = description,
                UserCreatorID = userCreatorID,
                CreatedDateTime = createdDateTime,
                IsPercentage = isPercentage,
                Discount = discount,
                IsMultipleDiscounts = isMultipleDiscounts,
                MaxUsageCount = maxUsageCount,
                ExpirationDate = expirationDate
            };

            // Insert the coupon into the database
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
        }

        // Delete a coupon by its code
        public async Task<bool> DeleteAsync(string couponCode)
        {
            var coupon = await _context.Coupons.FindAsync(couponCode);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
                return true; // Successfully deleted coupon
            }
            return false; // Coupon not found
        }

        // Update an existing coupon
        public async Task<bool> UpdateAsync(string couponCode, Coupons updatedCoupon)
        {
            if (updatedCoupon == null)
            {
                throw new ArgumentNullException(nameof(updatedCoupon), "Updated coupon cannot be null.");
            }

            var existingCoupon = await _context.Coupons.FindAsync(couponCode);
            if (existingCoupon == null)
            {
                return false; // Coupon not found, cannot update
            }

            // Update the coupon details
            existingCoupon.Description = updatedCoupon.Description;
            existingCoupon.UserCreatorID = updatedCoupon.UserCreatorID;
            existingCoupon.CreatedDateTime = updatedCoupon.CreatedDateTime;
            existingCoupon.IsPercentage = updatedCoupon.IsPercentage;
            existingCoupon.Discount = updatedCoupon.Discount;
            existingCoupon.IsMultipleDiscounts = updatedCoupon.IsMultipleDiscounts;
            existingCoupon.MaxUsageCount = updatedCoupon.MaxUsageCount;
            existingCoupon.ExpirationDate = updatedCoupon.ExpirationDate;

            await _context.SaveChangesAsync();
            return true;
        }

        // Get a coupon by its code
        public async Task<Coupons> GetAsync(string couponCode)
        {
            return await _context.Coupons.FindAsync(couponCode);
        }

        // Retrieve all coupons
        public async Task<List<Coupons>> GetAllCouponsAsync()
        {
            return await _context.Coupons.ToListAsync();
        }
    }
}
