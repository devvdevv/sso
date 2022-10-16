using AuthorizationServer;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
Console.Title = "IdentityServer4 is running...";

// Add services to the container.
var services = builder.Services;
services.AddMvc();
services.AddIdentityServer(options =>
    {
        // Disable OIDC endpoints
        options.Endpoints.EnableCheckSessionEndpoint = false;
        options.Endpoints.EnableEndSessionEndpoint = false;
    })
    .AddInMemoryClients(IdentityConfiguration.Clients)
    .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
    .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
    .AddTestUsers(IdentityConfiguration.TestUsers)
    .AddDeveloperSigningCredential();
services.AddAuthentication().AddCookie("774a0068e9c04e97ba6a96f85f61c05c");
    
var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();
