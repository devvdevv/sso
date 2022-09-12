using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sso.Data;

namespace sso;

[ApiController]
[Route("[controller]")]
[Authorize]
public class WeatherForeCastController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        var service = new WeatherForecastService();
        var result = service.GetForecastAsync(DateTime.Now);
        return Ok(result);
    }
}