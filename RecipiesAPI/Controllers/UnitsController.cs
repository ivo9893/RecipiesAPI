using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipiesAPI.Services;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Controllers
{

    
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitsService _unitsService;

        public UnitsController(IUnitsService unitsService)
        {
            _unitsService = unitsService;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUnits()
        {
            try
            {
                var units = await _unitsService.GetAllUnitsAsync();
                return Ok(units);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
