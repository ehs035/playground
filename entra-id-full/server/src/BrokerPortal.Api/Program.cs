using BrokerPortal.Api.Authorization;
using BrokerPortal.Api.Cache;
using BrokerPortal.Api.Data;
using BrokerPortal.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserClaimsRepository, DapperUserClaimsRepository>();
builder.Services.AddSingleton<IUserContextCache, MemoryUserContextCache>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<ClaimEnricher>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        jwtOptions =>
        {
            builder.Configuration.Bind("AzureAd", jwtOptions);
            jwtOptions.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var enricher = context.HttpContext.RequestServices.GetRequiredService<ClaimEnricher>();
                    await enricher.EnrichAsync(context.Principal!, context.HttpContext, context.HttpContext.RequestAborted);
                }
            };
        },
        identityOptions =>
        {
            builder.Configuration.Bind("AzureAd", identityOptions);
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AgencyAccess", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new AgencyAccessRequirement());
    });

    options.AddPolicy("BrokerOfAgency", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new BrokerOfAgencyRequirement());
    });
});

builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, AgencyAccessHandler>();
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, BrokerOfAgencyHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
