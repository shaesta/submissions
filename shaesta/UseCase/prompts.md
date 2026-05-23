# Prompts Used

1. Clarifying question to user:
   - "Which .NET SDK should I target for the API (7.0 or 8.0)? For the React frontend, prefer Vite or Create React App (CRA)?"

2. Scaffolding prompt (used to generate project skeleton and files):
   - "Create a .NET 8 minimal API project with IFlightStatusProvider interface, two stub providers (AeroTrack and QuickFlight), a Normalizer service implementing mapping rules, and an endpoint GET /flights/status that queries providers in parallel and returns a unified FlightStatusResult. Also scaffold an xUnit test project with tests for mapping and provider selection. Finally scaffold a Vite React frontend with a simple search form. Be deterministic and include run instructions."

Notes: Copilot assistance was used to accelerate code generation, especially for repetitive boilerplate (project files, minimal API wiring, test scaffolding, and frontend component structure). Where Copilot suggestions were accepted, I reviewed and adjusted mappings and deterministic stub data to match the spec.
