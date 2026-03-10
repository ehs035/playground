using FastEndpoints;
using FastEndpoints.Swagger;
using FeatureTelemetry.SampleApi.Middleware;
using FeatureTelemetry.SampleApi.Services;
using FeatureTelemetry.SampleApi.Telemetry;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext().WriteTo.Console());

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<ITelemetryContextAccessor, TelemetryContextAccessor>();
builder.Services.AddSingleton<IFeatureTelemetryMetadataResolver, FeatureTelemetryMetadataResolver>();
builder.Services.AddSingleton<IFeatureTelemetrySummaryStore, InMemoryFeatureTelemetrySummaryStore>();
builder.Services.AddSingleton<IFeatureTelemetryRepository, ConsoleFeatureTelemetryRepository>();
builder.Services.AddSingleton<IFeatureTelemetry, FeatureTelemetryService>();
builder.Services.AddHostedService<FeatureTelemetryAggregationWorker>();
builder.Services.AddSingleton<IProviderSearchService, ProviderSearchService>();
builder.Services.AddSingleton<IPhiSubmissionService, PhiSubmissionService>();

var allowedOrigins = new[]
{
    "https://localhost:56281",
    "http://localhost:56281"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("TelemetryDashboardCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors("TelemetryDashboardCors");

app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseFastEndpoints();
app.MapControllers();
app.UseSwaggerGen();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();