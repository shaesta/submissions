using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;

namespace FlightStatus.Tests;

public class NormalizerTests
{
    [Fact]
    public void Maps_AeroTrack_Delayed_To_Delayed()
    {
        var normalizer = new Normalizer();
        var resp = new ProviderResponse
        {
            ProviderName = "AeroTrack",
            Success = true,
            LastUpdatedUtc = DateTime.Parse("2026-05-23T12:00:00Z"),
            Data = new ProviderData
            {
                RawStatus = "DELAYED",
                ScheduledDepartureUtc = DateTime.Parse("2026-05-23T10:00:00Z"),
                ActualDepartureUtc = DateTime.Parse("2026-05-23T10:40:00Z")
            }
        };

        var result = normalizer.Normalize(new[] { resp }, "A1-100", DateTime.Parse("2026-05-23"));
        Assert.Equal(FlightStatus.Api.Models.UnifiedStatus.Delayed, result.Status);
    }

    [Fact]
    public void Chooses_Later_LastUpdated_Response()
    {
        var normalizer = new Normalizer();
        var r1 = new ProviderResponse
        {
            ProviderName = "AeroTrack",
            Success = true,
            LastUpdatedUtc = DateTime.Parse("2026-05-23T11:00:00Z"),
            Data = new ProviderData { RawStatus = "ONTIME", ScheduledDepartureUtc = DateTime.Parse("2026-05-23T09:00:00Z"), ActualDepartureUtc = DateTime.Parse("2026-05-23T09:10:00Z") }
        };
        var r2 = new ProviderResponse
        {
            ProviderName = "QuickFlight",
            Success = true,
            LastUpdatedUtc = DateTime.Parse("2026-05-23T12:00:00Z"),
            Data = new ProviderData { RawStatus = "LATE", ScheduledDepartureUtc = DateTime.Parse("2026-05-23T09:00:00Z") }
        };

        var result = normalizer.Normalize(new[] { r1, r2 }, "Q1-200", DateTime.Parse("2026-05-23"));
        Assert.Equal(FlightStatus.Api.Models.UnifiedStatus.Delayed, result.Status);
        Assert.Equal("QuickFlight", result.Provider);
    }
}
