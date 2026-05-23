using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

public class QuickFlightProvider : IFlightStatusProvider
{
    public string Name => "QuickFlight";

    public Task<ProviderResponse> GetFlightStatusAsync(string flightNumber, DateTime date, CancellationToken ct)
    {
        // Deterministic stubbed responses
        var resp = new ProviderResponse { ProviderName = Name, Success = true };

        if (flightNumber.Contains("Q1", StringComparison.OrdinalIgnoreCase))
        {
            resp.LastUpdatedUtc = DateTime.Parse("2026-05-23T12:30:00Z");
            resp.Data = new ProviderData
            {
                RawStatus = "LATE",
                ScheduledDepartureUtc = DateTime.Parse("2026-05-23T10:00:00Z"),
                ScheduledArrivalUtc = DateTime.Parse("2026-05-23T12:00:00Z")
            };
        }
        else if (flightNumber.Contains("ON", StringComparison.OrdinalIgnoreCase))
        {
            resp.LastUpdatedUtc = DateTime.Parse("2026-05-23T11:30:00Z");
            resp.Data = new ProviderData
            {
                RawStatus = "ON_TIME",
                ScheduledDepartureUtc = DateTime.Parse("2026-05-23T09:00:00Z"),
                ScheduledArrivalUtc = DateTime.Parse("2026-05-23T11:00:00Z")
            };
        }
        else
        {
            resp.Success = false;
            resp.LastUpdatedUtc = DateTime.Parse("2026-05-23T09:05:00Z");
        }

        resp.Raw = new { sample = "quickflight-stub" };
        return Task.FromResult(resp);
    }
}
