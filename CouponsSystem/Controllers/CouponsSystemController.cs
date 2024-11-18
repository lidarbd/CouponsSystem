using Microsoft.AspNetCore.Mvc;
using System;
using CouponsSystem.Data;
using CouponsSystem.Models;


namespace CouponsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsSystemController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly CouponsManager _couponsManager;
        private readonly Reports _reports;

        public CouponsSystemController(AppDbContext appDbContext, CouponsManager couponsManager, Reports reports)
        {
            _appDbContext = appDbContext;
            _couponsManager = couponsManager;
            _reports = reports;
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

