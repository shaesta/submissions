using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFlightStatusProvider, AeroTrackProvider>();
builder.Services.AddSingleton<IFlightStatusProvider, QuickFlightProvider>();
builder.Services.AddSingleton<INormalizer, Normalizer>();
// Enable CORS for development
builder.Services.AddCors(options => options.AddPolicy("Dev", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// Use the Dev CORS policy
app.UseCors("Dev");

app.MapGet("/flights/status", async (HttpContext http, IEnumerable<IFlightStatusProvider> providers, INormalizer normalizer, CancellationToken ct) =>
{
    var q = http.Request.Query;
    if (!q.TryGetValue("flightNumber", out var fn) || string.IsNullOrWhiteSpace(fn))
        return Results.BadRequest(new { error = "flightNumber is required" });
    if (!q.TryGetValue("date", out var dateStr) || !DateTime.TryParse(dateStr, out var date))
        return Results.BadRequest(new { error = "date is required and must be yyyy-MM-dd" });

    var tasks = providers.Select(p => p.GetFlightStatusAsync(fn!, date, ct)).ToArray();
    var responses = await Task.WhenAll(tasks);

    var result = normalizer.Normalize(responses.Where(r => r != null).ToArray(), fn!, date);
    return Results.Ok(result);
});

app.Run();
