using BrokerPortal.Api.Authorization;
using BrokerPortal.Api.Cache;
using BrokerPortal.Api.Data;
using BrokerPortal.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// CORS: allow your frontend dev server(s)
builder.Services.AddCors(options =>
{
	options.AddPolicy("FrontendDevCors", policy =>
	{
		policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

// Cache for agency→brokers (string-based)
builder.Services.AddSingleton<IAgencyBrokerCache, MemoryAgencyBrokerCache>();

// Data/Services
builder.Services.AddScoped<IUserClaimsRepository, DapperUserClaimsRepository>();
builder.Services.AddSingleton<IUserContextCache, MemoryUserContextCache>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<ClaimEnricher>();

// Authentication: Microsoft Entra ID
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

// Handlers should be Scoped (they depend on scoped repo)
builder.Services.AddScoped<IAuthorizationHandler, AgencyAccessHandler>();
builder.Services.AddScoped<IAuthorizationHandler, BrokerOfAgencyHandler>();


// Registers ISwaggerProvider and related services
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "BrokerPortal API",
		Version = "v1"
	});
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "BrokerPortal API v1");
		c.RoutePrefix = "swagger"; // UI at /swagger
	});
}



app.UseHttpsRedirection();

// CORS before auth for preflight OPTIONS
app.UseCors("FrontendDevCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/ping", () => Results.Ok("pong"));

app.Run();