using ChatBridge.Chat.API.Hubs;
using ChatBridge.Chat.Application.Interfaces;
using ChatBridge.Chat.Infrastructure.Data;
using ChatBridge.Chat.Infrastructure.GlobalTimer;
using ChatBridge.Chat.Infrastructure.Providers;
using ChatBridge.Chat.Infrastructure.Repositories;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ChatBridge.Chat.Infrastructure.Messaging.Publishers;
using ChatBridge.Chat.Infrastructure.Messaging.Interfaces;
using ChatBridge.Chat.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<RateLimitNotificationCooldown>();

// Register database context
builder.Services.AddDbContext<ChatBridgeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ChatBridgeConnection")));

// Register application interfaces and implementations

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IVendorProfileProvider, VendorProfileProvider>();
builder.Services.AddSingleton<GlobalMessageCooldown>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        RateLimitNotificationCooldown cooldown =
            context.HttpContext.RequestServices
                .GetRequiredService<RateLimitNotificationCooldown>();

        // Only the first rejected request during the cooldown publishes.
        if (!cooldown.TryAcquireNotification())
        {
            return;
        }

        IRabbitMQPublisher publisher =
            context.HttpContext.RequestServices
                .GetRequiredService<IRabbitMQPublisher>();

        ILogger<Program> logger =
            context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

        try
        {
            await publisher.PublishAsync(new
            {
                EventType = "RateLimitExceeded",
                Message = "ChatBridge rate limit exceeded.",
                OccurredAtUtc = DateTime.UtcNow
            });
        }
        catch (Exception exception)
        {
            // Allow another attempt if publishing fails.
            cooldown.ReleaseNotification();

            logger.LogError(
                exception,
                "Failed to publish rate-limit notification.");
        }
    };

    options.AddFixedWindowLimiter("MessageRateLimit", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromSeconds(90);
        limiter.QueueLimit = 0;
        limiter.AutoReplenishment = true;
    });
});

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

string rabbitMQConnectionString =
    builder.Configuration["RabbitMQ:ConnectionString"]
    ?? throw new InvalidOperationException(
        "RabbitMQ connection string is missing.");

ConnectionFactory connectionFactory = new ConnectionFactory
{
    Uri = new Uri(rabbitMQConnectionString)
};

IConnection rabbitMQConnection =
    await connectionFactory.CreateConnectionAsync();
builder.Services.AddSingleton<IConnection>(rabbitMQConnection);
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

var app = builder.Build();
app.MapGet("/api/chat/rabbitmq-test", async (
    IConnection connection) =>
{
    return Results.Ok(new
    {
        Connected = connection.IsOpen,
        Message = "RabbitMQ connection successful!"
    });
});

app.MapPost("/api/chat/rate-limit-test", () =>
{
    return Results.Ok("Request successful!");
})
.RequireRateLimiting("MessageRateLimit");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseRouting();

app.UseCors("AllowChatBridgeFrontend");
app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();
app.MapGet("/api/chat/test", () =>
{
    return Results.Ok("ChatBridge Chat API is working!");
});

app.MapHub<ChatHub>("/chatHub");
app.Run();
