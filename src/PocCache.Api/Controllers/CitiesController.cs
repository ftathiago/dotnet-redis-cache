using Microsoft.AspNetCore.Mvc;
using PocCache.Domain;

namespace PocCache.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICities _cities;

    public CitiesController(ICities cities)
    {
        _cities = cities;
    }

    [HttpGet]
    public async Task<IEnumerable<City>> GetAsync()
    {
        return await _cities.GetCitiesAsync();
    }
}
