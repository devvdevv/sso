using System.Net;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using WebClient.Data;

namespace WebClient.Controllers;

public class WeatherForecastApiController : Controller
{
    private const string ClientId = "simple_client";
    private const string ClientSecret = "secret";
    private const string RedirectUri = "http://localhost:44335/callback";
    private static string Message { get; set; } = "";
    private static string? Code { get; set; }
    private static string? Token { get; set; }

    public IActionResult Index()
    {
        return View("Index", Message);
    }

    public IActionResult Authorize()
    {
        Message += "\n\nRedirecting to authorization endpoint...";
        return Redirect(
            $"http://localhost:44339/connect/authorize?client_id={ClientId}&scope=myApi.read.rewards&redirect_uri={RedirectUri}&response_type=code&response_mode=query");
    }

    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        Code = code;
        Message += "\nApplication Authorized!";

        Message += "\n\nCalling token endpoint...";
        var tokenClient = new HttpClient();
        var tokenResponse = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
        {
            Address = "https://localhost:44339/connect/token",
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            Code = Code,
            RedirectUri = RedirectUri
        });

        if (tokenResponse.IsError)
        {
            Message += "\nToken request failed";
            return RedirectToAction("Index");
        }

        Token = tokenResponse.AccessToken;
        Message += "\nToken Received!";

        return RedirectToAction("Index");
    }

    public async Task<WeatherForecast[]> GetWeatherForecast()
    {
        var httpClient = new HttpClient();
        if (Token != null) httpClient.SetBearerToken(Token);

        var response = await httpClient.GetAsync("https://localhost:44334/api/v1/weather-forecast");

        if (response.IsSuccessStatusCode)
            Message += "\n\nAPI access authorized!";
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
            Message += "\nUnable to contact API: Unauthorized!";
        else
            Message += $"\n\nUnable to contact API. Status code {response.StatusCode}";

        Console.WriteLine(response);

        return Array.Empty<WeatherForecast>();
    }
}