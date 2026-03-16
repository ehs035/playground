# BrokerPortal API (.NET 9)

ASP.NET Core 9 Web API protected by Microsoft Entra ID.

## What this solution does

- Validates Microsoft Entra access tokens with `Microsoft.Identity.Web`
- Reads the caller's `oid` claim
- Loads app-specific agency/broker context from SQL
- Uses in-memory cache with a cache-aside pattern
- Enriches the authenticated principal with app claims
- Enforces authorization through policy handlers
- Does **not** mint a second custom JWT

## Why this version matches the recommendation

This project follows the simpler and stronger default architecture:

- Entra ID issues the access token
- The API validates the token
- SQL remains the source of truth for agency/broker relationships
- Policies enforce authorization
- No extra internal JWT layer is added

## Setup

1. Update `src/BrokerPortal.Api/appsettings.json`
   - `AzureAd:TenantId`
   - `AzureAd:ClientId`
   - `AzureAd:Audience`
   - `ConnectionStrings:DefaultConnection`
2. Run `src/BrokerPortal.Api/sql/schema.sql` against SQL Server.
3. Replace the sample `Users.Oid` values with real Entra object IDs.
4. Restore and run:

```bash
dotnet restore
dotnet run --project src/BrokerPortal.Api
```

## Test flow

Acquire an Entra access token for this API from your client app and call:

- `GET /api/auth/me`
- `GET /api/agencies/{agencyId}`
- `GET /api/agencies/{agencyId}/brokers/{brokerId}`

## Main components

- `Program.cs` - auth, DI, Swagger, policies
- `Services/ClaimEnricher.cs` - attaches DB/cache-derived claims after token validation
- `Services/UserContextService.cs` - per-request + cache-aside orchestration
- `Cache/MemoryUserContextCache.cs` - in-memory cache + anti-stampede lock
- `Data/DapperUserClaimsRepository.cs` - SQL lookups with Dapper
- `Authorization/*` - policy requirements and handlers

## Notes

- This sample uses `IMemoryCache`. For multi-instance deployments, replace it with Redis-backed `IDistributedCache`.
- The current handlers are designed for MVC/controller authorization and route-based resources.
- If you later need downstream API calls, prefer Entra On-Behalf-Of rather than minting your own JWT.
