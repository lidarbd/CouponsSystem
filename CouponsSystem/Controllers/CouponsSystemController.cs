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

        // Create a new admin user
        [HttpPost("createAdminUser")]
        public async Task<IActionResult> CreateAdminUser([FromBody] AdminUserCreationDto adminUserDto)
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
                await _userSystem.CreateAdminUserAsync(adminUserDto.Id, adminUserDto.Username, adminUserDto.Password);
                return Ok("Admin user created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating admin user: {ex.Message}");
            }
        }

        // Log in a user
        [HttpPost("logInUser")]
        public async Task<IActionResult> LogInUser([FromBody] UserLoginDto userLoginDto)
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
        public IActionResult LogOutUser([FromBody] int userId)
        {
            if (userId == 0) // Check if the userId is zero, which is an invalid value
            {
                return BadRequest("User ID is required.");
            }

            // Convert the userId to string for the HashSet
            bool isLoggedOut = _userSystem.LogOutUser(userId.ToString());

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

            try
            {
                await _couponsManager.InsertAsync(
                    couponDto.Description,
                    couponDto.Code,
                    couponDto.UserCreatorID,
                    couponDto.CreatedDateTime,
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

            var updatedCoupon = new Coupon
            {
                Code = couponCode,
                Description = couponDto.Description,
                UserCreatorID = couponDto.UserCreatorID,
                CreatedDateTime = couponDto.CreatedDateTime,
                IsPercentage = couponDto.IsPercentage,
                Discount = couponDto.Discount,
                IsMultipleDiscounts = couponDto.IsMultipleDiscounts,
                MaxUsageCount = couponDto.MaxUsageCount,
                ExpirationDate = couponDto.ExpirationDate
            };

            bool isUpdated = await _couponsManager.UpdateAsync(couponCode, updatedCoupon);
            if (isUpdated)
            {
                return Ok("Coupon updated successfully.");
            }
            return NotFound("Coupon not found.");
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
                double finalPrice = await _couponsManager.CalculateFinalPriceAsync(couponCodes);
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

