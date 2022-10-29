using System.Net;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebClient.Data;

namespace WebClient.Controllers;

public class HomeController : Controller
{
    private const string ClientId = "cwm.client";
    private const string ClientSecret = "secret";
    private const string RedirectUri = "https://localhost:44336/callback";
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
            $"https://localhost:44335/connect/authorize?client_id={ClientId}&scope=myApi.read&redirect_uri={RedirectUri}&response_type=code&response_mode=query");
    }

    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        Code = code;
        Message += "\nApplication Authorized!";

        Message += "\n\nCalling token endpoint...";
        var tokenClient = new HttpClient();
        var tokenResponse = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
        {
            Address = "https://localhost:44335/connect/token",
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

    public async Task<IActionResult> GetWeatherForecast()
    {
        var httpClient = new HttpClient();
        if (Token != null) httpClient.SetBearerToken(Token);

        var response = await httpClient.GetAsync("https://localhost:44334/api/v1/weather-forecast");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            Message += $"\n\n {JsonConvert.DeserializeObject(data)}";
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            Message += "\nUnable to contact API: Unauthorized!";
        }
        else
        {
            Message += $"\n\nUnable to contact API. Status code {response.StatusCode}";
        }

        return RedirectToAction("Index");
    }
}