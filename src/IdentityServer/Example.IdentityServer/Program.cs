using AspNetCoreRateLimit;
using Example.Configurations.Hosting;
using Example.Configurations.Logging;
using Example.IdentityServer.Configurations;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.AddApplicationOptions();

builder.AddDatabase();

builder.ConfigureCorsPolicy();

builder.ConfigureKestrel();

builder.Host.UseSerilog();

builder.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();

builder.AddRateLimiting();

builder.ConfigureIdentityServerCors();

builder.ConfigureIdentityServerFunctionality();

var app = builder.Build();

app.MapHealthChecks("/_health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseClientRateLimiting();
app.UseHsts();
app.UseHttpsRedirection();
app.UseIdentityServer();
app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.SeedData();

app.Run();