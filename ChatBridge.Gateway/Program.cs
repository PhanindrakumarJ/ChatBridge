using ChatBridge.Gateway.Middleware;
using ChatBridge.Gateway.Services;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowChatBridgeFrontend", policy =>
    {
        policy
            .WithOrigins(
                "https://yellow-meadow-044b37b00.3.azurestaticapps.net",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services to the container.
builder.Configuration.AddJsonFile(
    "ocelotGateway.json",
    optional: false,
    reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<TraceLiteTelemetryService>(
    (serviceProvider, client) =>
    {
        IConfiguration configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        string? baseUrl =
            configuration["TraceLite:BaseUrl"];

        if (!Uri.TryCreate(
            baseUrl,
            UriKind.Absolute,
            out Uri? traceLiteUri))
        {
            throw new InvalidOperationException(
                "TraceLite BaseUrl is missing or invalid.");
        }

        client.BaseAddress = traceLiteUri;
        client.Timeout = TimeSpan.FromSeconds(2);
    });
builder.Services.AddSingleton<TraceLiteTelemetryQueue>();
builder.Services.AddHostedService<TraceLiteBackgroundWorker>();

var app = builder.Build();
app.UseCors("AllowChatBridgeFrontend");

app.UseWebSockets();

app.UseMiddleware<TraceLiteMiddleware>();


await app.UseOcelot();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
