using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

public class AeroTrackProvider : IFlightStatusProvider
{
    public string Name => "AeroTrack";

    public Task<ProviderResponse> GetFlightStatusAsync(string flightNumber, DateTime date, CancellationToken ct)
    {
        // Deterministic stubbed responses
        var resp = new ProviderResponse { ProviderName = Name, Success = true };

        if (flightNumber.Contains("A1", StringComparison.OrdinalIgnoreCase))
        {
            resp.LastUpdatedUtc = DateTime.Parse("2026-05-23T12:00:00Z");
            resp.Data = new ProviderData
            {
                RawStatus = "DELAYED",
                ScheduledDepartureUtc = DateTime.Parse("2026-05-23T10:00:00Z"),
                ActualDepartureUtc = DateTime.Parse("2026-05-23T10:40:00Z"),
                ScheduledArrivalUtc = DateTime.Parse("2026-05-23T12:00:00Z"),
                ActualArrivalUtc = DateTime.Parse("2026-05-23T12:40:00Z"),
                Terminal = "T1",
                Gate = "G5",
                DelayReason = "Technical"
            };
        }
        else if (flightNumber.Contains("ON", StringComparison.OrdinalIgnoreCase))
        {
            resp.LastUpdatedUtc = DateTime.Parse("2026-05-23T11:00:00Z");
            resp.Data = new ProviderData
            {
                RawStatus = "ONTIME",
                ScheduledDepartureUtc = DateTime.Parse("2026-05-23T09:00:00Z"),
                ActualDepartureUtc = DateTime.Parse("2026-05-23T09:10:00Z"),
                ScheduledArrivalUtc = DateTime.Parse("2026-05-23T11:00:00Z"),
                ActualArrivalUtc = DateTime.Parse("2026-05-23T10:58:00Z"),
                Terminal = "T2",
                Gate = "G1"
            };
        }
        else
        {
            resp.Success = false;
            resp.LastUpdatedUtc = DateTime.Parse("2026-05-23T09:00:00Z");
        }

        resp.Raw = new { sample = "aerotrack-stub" };
        return Task.FromResult(resp);
    }
}
