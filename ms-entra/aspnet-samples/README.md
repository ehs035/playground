# EntraSample (ASP.NET Core 9)

This sample shows how to protect endpoints using Microsoft Entra (Azure AD) and control access by roles, groups and a custom user attribute. It includes:

- Classic controller endpoints in `Controllers/SampleController.cs`
- FastEndpoints examples in `FastEndpoints/*`
- Swagger UI configured for OAuth2 (Authorization Code + PKCE)

Setup

1. Register an API app in Entra ID and expose a scope (e.g. `access_as_user`).
2. Configure the API app to include `groups` in tokens (or use group assignment checks).
3. If you need a custom attribute, create an extension attribute (Azure AD extension attribute) and expose it as an optional claim so it appears in the token (claim name looks like `extension_<guid>_CustomAttribute`) — the sample uses `extension_CustomAttribute` for brevity; replace with the real claim name.
4. Update `appsettings.json` with your `TenantId`, `ClientId`, and `Sample:GroupId` (group object id used for the group policy).

Run

```bash
dotnet restore
dotnet run --project aspnet-samples
```

Browse to `https://localhost:5001/swagger` (or the HTTPS URL shown) and use the OAuth2 button to sign in and acquire a token for the API scope.

Notes

- The controller demonstrates `[Authorize(Policy = "RequireAdminRole")]`, `[Authorize(Policy = "RequireGroup")]`, and `[Authorize(Policy = "RequireCustomAttribute")]`.
- The FastEndpoints versions use the same policy names via `Policies("PolicyName")`.
 - Replace claim names and scope identifiers with the values from your Entra app registration and optional claims configuration.

Getting a JWT for testing

 - Interactive (Authorization Code): use the Swagger UI `Authorize` button (recommended) or an OAuth2 client to request the scope configured in `appsettings.json` (`AzureAd:Scope`).
 - Client credentials (machine-to-machine): use the v2 token endpoint to request a token for your API scope. Example (replace placeholders):

```bash
curl -X POST \
	-H "Content-Type: application/x-www-form-urlencoded" \
	-d "client_id=YOUR_CLIENT_ID" \
	-d "scope=api://YOUR_API_CLIENT_ID/.default" \
	-d "client_secret=YOUR_CLIENT_SECRET" \
	-d "grant_type=client_credentials" \
	"https://login.microsoftonline.com/YOUR_TENANT_ID/oauth2/v2.0/token"
```

 - Authorization code (to get user claims like `groups` and extension attributes): follow the OAuth2 authorization code flow to obtain a token for the user. You can use MSAL, Postman, or the Swagger UI to sign-in interactively.

Local JWT (development only)

If you need a quick local JWT for testing (not recommended for production), you can create a signed JWT using a tool or script and include the `groups`, `roles`, and `extension_CustomAttribute` claims. Point your requests at the API with `Authorization: Bearer <token>` and use a validation setup that trusts the signing key (this sample is configured to use Entra ID tokens by default).

Inspecting claims

 - Use the new `GET /api/me` endpoint to see the authenticated user's `roles`, `groups`, `customAttribute`, and all token claims.
