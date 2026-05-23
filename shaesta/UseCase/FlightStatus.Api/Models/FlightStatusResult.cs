using System.Text.Json.Serialization;

namespace FlightStatus.Api.Models;

public record FlightStatusResult(
    string FlightNumber,
    string Date,
    UnifiedStatus Status)
{
    public DateTime? ScheduledDepartureUtc { get; init; }
    public DateTime? ActualDepartureUtc { get; init; }
    public DateTime? ScheduledArrivalUtc { get; init; }
    public DateTime? ActualArrivalUtc { get; init; }
    public string? Terminal { get; init; }
    public string? Gate { get; init; }
    public string? DelayReason { get; init; }
    public string? Provider { get; init; }
    public DateTime? LastUpdatedUtc { get; init; }
    public string? Message { get; init; }
}
