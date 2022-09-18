using AuthorizationServer;

var builder = WebApplication.CreateBuilder(args);
Console.Title = "IdentityServer4 is running...";

// Add services to the container.
var services = builder.Services;
services.AddMvc();
services.AddRazorPages();
services.AddIdentityServer()
    .AddInMemoryClients(IdentityConfiguration.Clients)
    .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
    .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
    .AddTestUsers(IdentityConfiguration.TestUsers)
    .AddDeveloperSigningCredential();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();

app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});

app.Run();
