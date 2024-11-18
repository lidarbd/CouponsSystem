using Microsoft.AspNetCore.Mvc;
using System;
using CouponsSystem.Data;
using CouponsSystem.Models;
using CouponsSystem.DTO;


namespace CouponsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsSystemController : ControllerBase
    {
        private readonly UserSystem _userSystem;
        private readonly CouponsManager _couponsManager;
        private readonly Reports _reports;

        public CouponsSystemController(UserSystem userSystem, CouponsManager couponsManager, Reports reports)
        {
            _userSystem = userSystem;
            _couponsManager = couponsManager;
            _reports = reports;
        }

        // For testing
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        // Create a new admin user
        [HttpPost("createAdminUser")]
        public async Task<IActionResult> CreateAdminUser([FromBody] AdminUserDto adminUserDto)
        {
            if (adminUserDto == null)
            {
                return BadRequest("Admin user data is required.");
            }

            // Validate input data (basic checks)
            if (string.IsNullOrEmpty(adminUserDto.Username) || string.IsNullOrEmpty(adminUserDto.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            try
            {
                // Call UserSystem service to create the admin user
                await _userSystem.CreateAdminUserAsync(adminUserDto.Username, adminUserDto.Password);
                return Ok("Admin user created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating admin user: {ex.Message}");
            }
        }

        // Log in a user
        [HttpPost("logInUser")]
        public async Task<IActionResult> LogInUser([FromBody] AdminUserDto userLoginDto)
        {
            if (userLoginDto == null)
            {
                return BadRequest("Username and Password are required.");
            }

            var result = await _userSystem.LogInUserAsync(userLoginDto.Username, userLoginDto.Password);

            if (result)
            {
                return Ok("Login successful.");
            }
            else
            {
                return Unauthorized("Invalid username or password.");
            }
        }

        // Log out a user
        [HttpPost("logOutUser")]
        public async Task<IActionResult> LogOutUser([FromBody] int userId)
        {
            if (userId == 0) // Check if the userId is zero, which is an invalid value
            {
                return BadRequest("User ID is required.");
            }

            bool isLoggedOut = await _userSystem.LogOutUser(userId);

            if (isLoggedOut)
            {
                return Ok("User logged out successfully.");
            }
            else
            {
                return NotFound("User not found or already logged out.");
            }
        }


        // Insert a new coupon
        [HttpPost("createCoupon")]
        public async Task<IActionResult> CreateCoupon([FromBody] CouponCreationDto couponDto)
        {
            if (couponDto == null)
            {
                return BadRequest("Coupon data is required.");
            }

            var createdDateTime = DateTime.UtcNow;

            try
            {
                await _couponsManager.InsertAsync(
                    couponDto.Description,
                    couponDto.Code,
                    couponDto.UserCreatorID,
                    createdDateTime,
                    couponDto.IsPercentage,
                    couponDto.Discount,
                    couponDto.IsMultipleDiscounts,
                    couponDto.MaxUsageCount,
                    couponDto.ExpirationDate
                );
                return Ok("Coupon created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating coupon: {ex.Message}");
            }
        }

        // Get a coupon by code
        [HttpGet("getCoupon/{couponCode}")]
        public async Task<IActionResult> GetCoupon(string couponCode)
        {
            var coupon = await _couponsManager.GetAsync(couponCode);
            if (coupon == null)
            {
                return NotFound("Coupon not found.");
            }
            return Ok(coupon);
        }

        // Delete a coupon by code
        [HttpDelete("deleteCoupon/{couponCode}")]
        public async Task<IActionResult> DeleteCoupon(string couponCode)
        {
            bool isDeleted = await _couponsManager.DeleteAsync(couponCode);
            if (isDeleted)
            {
                return Ok("Coupon deleted successfully.");
            }
            return NotFound("Coupon not found.");
        }

        // Update an existing coupon
        [HttpPut("updateCoupon/{couponCode}")]
        public async Task<IActionResult> UpdateCoupon(string couponCode, [FromBody] CouponUpdateDto couponDto)
        {
            if (couponDto == null)
            {
                return BadRequest("Updated coupon data is required.");
            }

            // Retrieve the coupon by code
            var existingCoupon = await _couponsManager.GetAsync(couponCode);
            if (existingCoupon == null)
            {
                return NotFound("Coupon not found.");
            }

            // Update the existing coupon's properties
            existingCoupon.Description = couponDto.Description;
            existingCoupon.IsPercentage = couponDto.IsPercentage;
            existingCoupon.Discount = couponDto.Discount;
            existingCoupon.IsMultipleDiscounts = couponDto.IsMultipleDiscounts;
            existingCoupon.MaxUsageCount = couponDto.MaxUsageCount;
            existingCoupon.ExpirationDate = couponDto.ExpirationDate;

            // Save changes to the database
            bool isUpdated = await _couponsManager.UpdateAsync(couponCode, existingCoupon);
            if (isUpdated)
            {
                return Ok("Coupon updated successfully.");
            }

            return BadRequest("Error updating the coupon.");
        }

        [HttpPost("applyCoupons")]
        public async Task<IActionResult> ApplyCoupons([FromBody] List<string> couponCodes)
        {
            if (couponCodes == null || couponCodes.Count == 0)
            {
                return BadRequest("Please provide at least one coupon code.");
            }

            try
            {
                // Create a dictionary to count the frequency of each coupon code
                var couponFrequency = couponCodes.GroupBy(code => code)
                                                  .ToDictionary(group => group.Key, group => group.Count());

                double finalPrice = 100;  // Assume an initial order amount of 100 ¤

                foreach (var couponCode in couponCodes)
                {
                    var coupon = await _couponsManager.GetAsync(couponCode);

                    if (coupon == null)
                    {
                        return BadRequest($"Coupon with code {couponCode} not found.");
                    }

                    // Check if the coupon has expired
                    if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate.Value < DateTime.UtcNow)
                    {
                        return BadRequest($"Coupon {couponCode} has expired.");
                    }

                    // Step 3: Check if the coupon usage count is within the allowed limit
                    int usageCount = couponFrequency[couponCode];

                    if (coupon.MaxUsageCount.HasValue && usageCount > coupon.MaxUsageCount)
                    {
                        return BadRequest($"Coupon {couponCode} has exceeded its maximum usage limit.");
                    }

                    // Apply the coupon to the final price
                    finalPrice =  _couponsManager.CalculateFinalPrice(couponCode, finalPrice, coupon);
                }

                return Ok(new { FinalPrice = finalPrice });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error applying coupons: {ex.Message}");
            }
        }

        // Get coupons by a specific user
        [HttpGet("coupons/{userCreatorID}")]
        public async Task<IActionResult> GetCouponsByUser(int userCreatorID)
        {
            var coupons = await _reports.GetCouponsByUserAsync(userCreatorID);
            return Ok(coupons);
        }

        // Get coupons created within a specific date range
        [HttpGet("dateRange")]
        public async Task<IActionResult> GetCouponsByDateRange(DateTime startDate, DateTime endDate)
        {
            var coupons = await _reports.GetCouponsByDateRangeAsync(startDate, endDate);
            return Ok(coupons);
        }

        // Export coupons to an Excel file
        [HttpGet("exportToExcel")]
        public IActionResult ExportCouponsToExcel(DateTime? startDate, DateTime? endDate)
        {
            // Default to retrieving all coupons if no date range is provided
            List<Coupon> coupons;
            if (startDate.HasValue && endDate.HasValue)
            {
                coupons = _reports.GetCouponsByDateRangeAsync(startDate.Value, endDate.Value).Result;
            }
            else
            {
                coupons = _couponsManager.GetAllCouponsAsync().Result;
            }

            // Generate a temporary file path to save the Excel file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CouponsReport.xlsx");

            // Export the coupons to the file
            _reports.ExportCouponsToExcel(coupons, filePath);

            // Return the file as a download
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CouponsReport.xlsx");
        }
    }
}

