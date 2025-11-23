using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UrlShortener.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddSwaggerGen();

const string serviceName = "url-shortener-api";

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName))
        .AddConsoleExporter();
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());


var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();


app.MapPost("/api/v1/shorten", ([FromServices] ILogger<Program> logger, [FromBody] string url) =>
{
    logger.LogInformation("Shorten: {url}", url);
})
.WithName("Shorten");

app.MapGet("/{url}", ([FromServices] ILogger<Program> logger, string url) =>
{
    logger.LogInformation("Redirect: {url}", url);
})
.WithName("Redirect");

app.MapGet("/test", async () =>
{
    //var teste = RedisConnector.GetNextSeedNumber();
    await CassandraConnector.Connect();
});

app.Run();




