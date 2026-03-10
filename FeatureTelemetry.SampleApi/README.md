# Feature Telemetry Sample API

Sample ASP.NET Core project that demonstrates:
- FastEndpoints telemetry example
- Traditional MVC/Web API controller telemetry example
- Attribute-driven feature metadata
- Correlation ID middleware
- Console-only telemetry logging with Serilog

## Run

```bash
dotnet restore
dotnet run --project FeatureTelemetry.SampleApi
```

## Swagger
Open the swagger UI shown in the console output.

## FastEndpoints examples
- `POST /providers/search-fe`
- `POST /phi/submit-fe`

## Controller examples
- `POST /api/providers/search`
- `POST /api/phi/submit`

## Test payloads

### Provider search success
```json
{
  "query": "cardiology"
}
```

### Provider search timeout
```json
{
  "query": "timeout"
}
```

### Provider search error
```json
{
  "query": "error"
}
```

### PHI submit success
```json
{
  "memberId": "12345",
  "hasAttachments": true
}
```

### PHI submit timeout
```json
{
  "memberId": "timeout",
  "hasAttachments": false
}
```

### PHI submit error
```json
{
  "memberId": "error",
  "hasAttachments": false
}
```
