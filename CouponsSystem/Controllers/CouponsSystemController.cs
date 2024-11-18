using Microsoft.AspNetCore.Mvc;
using System;
using CouponsSystem.Data;


namespace CouponsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsSystemController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public CouponsSystemController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}

