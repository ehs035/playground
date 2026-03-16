using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        var instance = builder.Configuration["AzureAd:Instance"] ?? "https://login.microsoftonline.com/";
        var tenantId = builder.Configuration["AzureAd:TenantId"] ?? throw new InvalidOperationException("AzureAd:TenantId is not configured");
        var clientId = builder.Configuration["AzureAd:ClientId"] ?? throw new InvalidOperationException("AzureAd:ClientId is not configured");

        var authority = $"{instance}{tenantId}/v2.0";

        options.Authority = authority;
        options.Audience = clientId;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidAudiences = new[]
            {
                clientId
            },
            ValidIssuers = new[]
            {
                authority,
                $"{instance}{tenantId}/"
            }
        };

        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireGroup", policy => policy.RequireClaim("groups", builder.Configuration["Sample:GroupId"] ?? "GROUP_ID_PLACEHOLDER"));
    options.AddPolicy("RequireCustomAttribute", policy => policy.RequireClaim("extension_CustomAttribute", "Allowed"));
});

builder.Services.AddControllers();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EntraSample API", Version = "v1" });

    var oauthScheme = new OpenApiSecurityScheme
    {
        Name = "oauth2",
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string> { { builder.Configuration["AzureAd:Scope"] ?? "api://{clientId}/.default", "Access the API" } }
            }
        }
    };

    c.AddSecurityDefinition("oauth2", oauthScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { oauthScheme, new List<string> { builder.Configuration["AzureAd:Scope"] ?? string.Empty } }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EntraSample v1");
    c.OAuthClientId(builder.Configuration["AzureAd:ClientId"] ?? "CLIENT_ID");
    c.OAuthUsePkce();
});

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseFastEndpoints();

app.Run();
