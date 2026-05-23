# Flight Status — Specification

Author: shaesta
Date: 2026-05-23

## Goal

Define the data model, interfaces, provider response shapes, and normalization rules for the Flight Status lookup feature. This file is committed before any implementation per evaluator rules.

## Assumptions

- Flight numbers are airline code + number (free-form string). We require a non-empty string.
- Date uses `yyyy-MM-dd` and is treated as local date for the flight day's context.
- Providers are deterministic stubs in this project (no external network calls).
- When both providers return results, prefer the one with the later `lastUpdatedUtc` timestamp.
- If neither provider returns a usable status, API returns `Unknown` and an explanatory message.

## Unified Models

### Unified status enum

- `OnTime` — Departing/arrived within 15 minutes of schedule
- `Delayed` — Departure or arrival pushed beyond 15 minutes
- `Cancelled` — Flight will not operate
- `Diverted` — Flight landed at a different airport
- `Unknown` — Provider returned no usable status

### FlightStatusResult

- `flightNumber` (string)
- `date` (string, yyyy-MM-dd)
- `status` (UnifiedStatus)
- `scheduledDepartureUtc?` (DateTime?)
- `actualDepartureUtc?` (DateTime?)
- `scheduledArrivalUtc?` (DateTime?)
- `actualArrivalUtc?` (DateTime?)
- `terminal?` (string)
- `gate?` (string)
- `delayReason?` (string)
- `provider` (string) — the provider used (AeroTrack|QuickFlight)
- `lastUpdatedUtc?` (DateTime?)
- `message?` (string) — additional note when `Unknown` or when provider errors occur

## Provider abstraction

Define a single interface for providers:

IFlightStatusProvider

- `string Name { get; }`
- `Task<ProviderResponse> GetFlightStatusAsync(string flightNumber, DateTime date, CancellationToken ct)`

ProviderResponse (common wrapper)

- `bool Success`
- `string ProviderName`
- `DateTime? LastUpdatedUtc`
- `object Raw` — provider-specific payload for debugging
- `ProviderData? Data` — parsed provider-specific data (optional)

ProviderData (fields common to both providers after minimal parsing)

- `string RawStatus` — provider-specific status vocabulary
- `DateTime? ScheduledDepartureUtc`
- `DateTime? ActualDepartureUtc`
- `DateTime? ScheduledArrivalUtc`
- `DateTime? ActualArrivalUtc`
- `string? Terminal`
- `string? Gate`
- `string? DelayReason`

## Provider response shapes (stubs)

### AeroTrack (verbose)

- Fields (example):
  - `flight_code`, `status_code`, `sched_dep_utc`, `actual_dep_utc`, `sched_arr_utc`, `actual_arr_utc`, `terminal`, `gate`, `delay_reason`, `last_updated_utc`

### QuickFlight (minimal)

- Fields (example):
  - `flight`, `state`, `scheduled_dep_utc`, `scheduled_arr_utc`, `last_updated_utc`

## Normalisation rules

1. Map provider-specific `status`/`state`/`status_code` to the unified enum.
   - Provide explicit mapping tables in implementation. Examples:
     - AeroTrack: `ONTIME` -> `OnTime`, `DELAYED` -> `Delayed`, `CANCELLED` -> `Cancelled`, `DIVERTED` -> `Diverted`.
     - QuickFlight: `ON_TIME` -> `OnTime`, `LATE` -> `Delayed`, `NO_SERVICE` -> `Cancelled`, `REROUTED` -> `Diverted`.
   - If an unknown provider status is encountered, map to `Unknown` but include raw status in `message`.

2. Time-based determination for `OnTime` vs `Delayed`:
   - If both schedule and actual times are present, compute delay = |actual - scheduled|. If delay <= 15 minutes -> `OnTime` else `Delayed`.
   - If only scheduled time and provider reports `OnTime`/`On Time`/equivalent, map to `OnTime`.

3. Selection when both providers return results:
   - Compare `lastUpdatedUtc` from provider responses. Use the provider with the later `lastUpdatedUtc`.
   - If `lastUpdatedUtc` equal or missing, prefer AeroTrack (more detailed).

4. Partial fields:
   - Display AeroTrack-only fields (`terminal`, `gate`, `delayReason`) when present. They are optional in `FlightStatusResult`.

5. Failures:
   - If a provider returns an error or `Success == false`, ignore that provider but log the issue.
   - If both providers fail or return no usable status, return `FlightStatusResult` with `status = Unknown` and `message` describing the problem.

## API contract

GET /flights/status?flightNumber={code}&date={yyyy-MM-dd}

Responses:

- 200 OK: JSON `FlightStatusResult`
- 400 Bad Request: when `flightNumber` or `date` missing/invalid
- 500 Internal Server Error: for unexpected errors

## Test cases (to cover in unit tests)

- Mapping AeroTrack statuses to unified statuses (all enum variants)
- Mapping QuickFlight statuses to unified statuses
- Delay calculation boundary: exactly 15 minutes => OnTime
- Provider selection: later `lastUpdatedUtc` wins
- When only one provider returns, use that result
- When both providers fail, API returns `Unknown` with message

## Implementation notes

- Providers are registered in DI as `IFlightStatusProvider` (multiple implementations).
- The API endpoint receives `IEnumerable<IFlightStatusProvider>` and queries them in parallel with a timeout.
- Copilot usage: all significant prompts will be recorded in `prompts.md`.
