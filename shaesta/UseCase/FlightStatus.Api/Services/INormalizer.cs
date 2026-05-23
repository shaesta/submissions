using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;

namespace FlightStatus.Api.Services;

public interface INormalizer
{
    FlightStatusResult Normalize(ProviderResponse[] responses, string flightNumber, DateTime date);
}
