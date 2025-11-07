using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using WebApplicationWebApp.Models;

namespace WebApplicationWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDownstreamApi _downstreamApi;

        public HomeController(ILogger<HomeController> logger, IDownstreamApi downstreamApi)
        {
            _logger = logger;
            _downstreamApi = downstreamApi;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi")]
        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            WeatherForecast[]? forecasts = Array.Empty<WeatherForecast>();

            try
            {
                forecasts = await _downstreamApi.GetForUserAsync<WeatherForecast[]>("DemoApi");
            }
            catch (MicrosoftIdentityWebChallengeUserException challenge)
            {
                _logger.LogWarning(challenge, "User interaction required to consent to API scope.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call downstream API");
                ViewData["ApiError"] = ex.Message;
            }

            return View(forecasts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
