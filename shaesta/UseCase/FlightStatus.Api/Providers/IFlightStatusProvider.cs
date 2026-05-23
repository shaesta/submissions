using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

public class ProviderData
{
    public string? RawStatus { get; set; }
    public DateTime? ScheduledDepartureUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }
    public DateTime? ScheduledArrivalUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public string? Terminal { get; set; }
    public string? Gate { get; set; }
    public string? DelayReason { get; set; }
}

public class ProviderResponse
{
    public bool Success { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public DateTime? LastUpdatedUtc { get; set; }
    public object? Raw { get; set; }
    public ProviderData? Data { get; set; }
}

public interface IFlightStatusProvider
{
    string Name { get; }
    Task<ProviderResponse> GetFlightStatusAsync(string flightNumber, DateTime date, CancellationToken ct);
}
