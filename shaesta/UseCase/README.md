# Flight Status — Submission (shaesta)

## Overview

This project implements a Flight Status lookup with a .NET 8 Minimal API backend and a Vite React frontend. Providers are deterministic stubs.

## Run (backend)

Prerequisites: .NET 8 SDK

From repository root:

```powershell
cd submissions/shaesta/UseCase/FlightStatus.Api
dotnet run
```

API endpoint: `http://localhost:5000/flights/status?flightNumber={code}&date={yyyy-MM-dd}`

## Run (frontend)

Prerequisites: Node 18+, npm

```bash
cd submissions/shaesta/UseCase/flight-status-ui
npm install
npm run dev
```

The UI will call the backend at `http://localhost:5000`.

## Tests

```powershell
cd submissions/shaesta/UseCase/FlightStatus.Tests
dotnet test
```

## Notes and assumptions

- See `spec.md` for detailed models and normalization rules.
- Providers are stubs returning deterministic responses for flight numbers containing `A1`, `Q1`, or `ON` for testing.
