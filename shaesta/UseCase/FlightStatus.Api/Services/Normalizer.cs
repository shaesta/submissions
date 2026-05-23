using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;

namespace FlightStatus.Api.Services;

public class Normalizer : INormalizer
{
    public FlightStatusResult Normalize(ProviderResponse[] responses, string flightNumber, DateTime date)
    {
        var valid = responses.Where(r => r != null && r.Success && r.Data != null).ToArray();
        if (!valid.Any())
        {
            return new FlightStatusResult(flightNumber, date.ToString("yyyy-MM-dd"), UnifiedStatus.Unknown)
            {
                Message = "No provider returned usable status"
            };
        }

        ProviderResponse chosen;
        if (valid.Length == 1)
            chosen = valid[0];
        else
        {
            // prefer later lastUpdatedUtc
            chosen = valid.OrderByDescending(r => r.LastUpdatedUtc ?? DateTime.MinValue).First();
            // if equal, prefer AeroTrack
            var top = valid.OrderByDescending(r => r.LastUpdatedUtc ?? DateTime.MinValue).Take(2).ToArray();
            if (top.Length == 2 && (top[0].LastUpdatedUtc == top[1].LastUpdatedUtc))
            {
                var aero = valid.FirstOrDefault(r => r.ProviderName == "AeroTrack");
                if (aero != null) chosen = aero;
            }
        }

        var data = chosen.Data!;
        var status = MapStatus(chosen.ProviderName, data.RawStatus, data.ScheduledDepartureUtc, data.ActualDepartureUtc, data.ScheduledArrivalUtc, data.ActualArrivalUtc);

        return new FlightStatusResult(flightNumber, date.ToString("yyyy-MM-dd"), status)
        {
            ScheduledDepartureUtc = data.ScheduledDepartureUtc,
            ActualDepartureUtc = data.ActualDepartureUtc,
            ScheduledArrivalUtc = data.ScheduledArrivalUtc,
            ActualArrivalUtc = data.ActualArrivalUtc,
            Terminal = data.Terminal,
            Gate = data.Gate,
            DelayReason = data.DelayReason,
            Provider = chosen.ProviderName,
            LastUpdatedUtc = chosen.LastUpdatedUtc
        };
    }

    private UnifiedStatus MapStatus(string provider, string? rawStatus, DateTime? schedDep, DateTime? actualDep, DateTime? schedArr, DateTime? actualArr)
    {
        if (string.IsNullOrWhiteSpace(rawStatus)) return UnifiedStatus.Unknown;
        var rs = rawStatus.Trim().ToUpperInvariant();

        // provider-specific mappings
        if (provider == "AeroTrack")
        {
            return rs switch
            {
                "ONTIME" => UnifiedStatus.OnTime,
                "DELAYED" => UnifiedStatus.Delayed,
                "CANCELLED" => UnifiedStatus.Cancelled,
                "DIVERTED" => UnifiedStatus.Diverted,
                _ => TimeBasedDecision(schedDep, actualDep, schedArr, actualArr)
            };
        }

        if (provider == "QuickFlight")
        {
            return rs switch
            {
                "ON_TIME" => UnifiedStatus.OnTime,
                "LATE" => UnifiedStatus.Delayed,
                "NO_SERVICE" => UnifiedStatus.Cancelled,
                "REROUTED" => UnifiedStatus.Diverted,
                _ => TimeBasedDecision(schedDep, actualDep, schedArr, actualArr)
            };
        }

        return TimeBasedDecision(schedDep, actualDep, schedArr, actualArr);
    }

    private UnifiedStatus TimeBasedDecision(DateTime? schedDep, DateTime? actualDep, DateTime? schedArr, DateTime? actualArr)
    {
        var threshold = TimeSpan.FromMinutes(15);
        if (actualDep.HasValue && schedDep.HasValue)
        {
            var d = (actualDep.Value - schedDep.Value).Duration();
            return d <= threshold ? UnifiedStatus.OnTime : UnifiedStatus.Delayed;
        }
        if (actualArr.HasValue && schedArr.HasValue)
        {
            var d = (actualArr.Value - schedArr.Value).Duration();
            return d <= threshold ? UnifiedStatus.OnTime : UnifiedStatus.Delayed;
        }
        return UnifiedStatus.Unknown;
    }
}
