# Feature Telemetry Sample API with Aggregation

Includes:
- FastEndpoints examples
- MVC controller examples
- console telemetry logging
- in-memory aggregation worker
- summary endpoint

## Run
```bash
dotnet restore
dotnet run --project FeatureTelemetry.SampleApi
```

## Endpoints
- `POST /providers/search-fe`
- `POST /phi/submit-fe`
- `POST /api/providers/search`
- `POST /api/phi/submit`
- `GET /api/telemetry/summary`

- `GET /api/telemetry/events`
