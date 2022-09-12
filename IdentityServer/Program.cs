using IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
Console.Title = "IdentityServer4 is running...";
services.AddMvc();
services.AddIdentityServer()
    .AddInMemoryClients(IdentityConfiguration.Clients)
    .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
    .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
    .AddTestUsers(IdentityConfiguration.TestUsers)
    .AddDeveloperSigningCredential();

var app = builder.Build();
app.UseRouting();
app.UseIdentityServer();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});

app.Run();