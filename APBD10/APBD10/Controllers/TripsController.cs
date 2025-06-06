using APBD10.Data;
using APBD10.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD10.Controllers;
[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase
{
    private readonly DbService.ITripService _tripService;

    public TripsController(DbService.ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var tripsPaged = await _tripService.GetTripsAsync(page, pageSize);
        return Ok(tripsPaged);
    }
}