using FastEndpoints;
using FastEndpoints.Swagger;
using ProviderInbox.Service.Features.Inbox.Reprioritize;
using ProviderInbox.Service.Infrastructure;
using ProviderInbox.Service.Infrastructure.BackgroundJobs;
using ProviderInbox.Service.Seeds;
using ProviderInbox.Service.Shared.Security;
using ProviderInbox.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(s => s.DocumentSettings = d => { d.Title = "Provider Inbox Service"; d.Version = "v1"; });

// Aspire DB resource name: providerinboxdb (created in AppHost)
builder.AddSqlServerDbContext<AppDbContext>("providerinboxdb");

builder.Services.AddScoped<PriorityRulesEngine>();
builder.Services.AddScoped<IInboxReprioritizeService, InboxReprioritizeService>();

// v1 (dev): scope by querystring. Swap to ClaimsAudienceContext when JWT is enabled.
builder.Services.AddScoped<IAudienceContext, QueryStringAudienceContext>();

builder.Services.AddHostedService<PriorityReprioritizationWorker>();
builder.Services.AddHostedService<SeedDataHostedService>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();

public partial class Program { }
