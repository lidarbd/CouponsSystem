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
        private readonly AppDbContext _appDbContext;
        private readonly CouponsManager _couponsManager;
        private readonly Reports _reports;
        private readonly UserSystem _userSystem;

        public CouponsSystemController(AppDbContext appDbContext, CouponsManager couponsManager, Reports reports, UserSystem userSystem)
        {
            _appDbContext = appDbContext;
            _couponsManager = couponsManager;
            _reports = reports;
            _userSystem = userSystem;
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

            // Convert the userId to string for the HashSet, assuming it stores user IDs as strings
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


        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _couponsManager.GetAllCouponsAsync();
            return Ok(coupons);
        }
    

        [HttpGet("user/{userCreatorID}")]
        public async Task<IActionResult> GetCouponsByUser(int userCreatorID)
        {
            var coupons = await _reports.GetCouponsByUserAsync(userCreatorID);
            return Ok(coupons);
        }

        [HttpGet("dateRange")]
        public async Task<IActionResult> GetCouponsByDateRange(DateTime startDate, DateTime endDate)
        {
            var coupons = await _reports.GetCouponsByDateRangeAsync(startDate, endDate);
            return Ok(coupons);
        }
    }
}

