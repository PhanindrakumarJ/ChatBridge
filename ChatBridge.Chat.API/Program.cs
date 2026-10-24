using ChatBridge.Chat.Application.Interfaces;
using ChatBridge.Chat.Infrastructure.Data;
using ChatBridge.Chat.Infrastructure.Providers;
using ChatBridge.Chat.Infrastructure.Repositories;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();

// Register database context
builder.Services.AddDbContext<ChatBridgeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ChatBridgeConnection")));

// Register application interfaces and implementations

builder.Services.AddScoped<IChatRepository, ChatRepository>();

builder.Services.AddScoped<IVendorProfileProvider, VendorProfileProvider>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("MessageRateLimit", limiter =>
    {
        limiter.PermitLimit = 3;
        limiter.Window = TimeSpan.FromSeconds(90);
        limiter.QueueLimit = 0;
        limiter.AutoReplenishment = true;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/api/chat/test", () =>
{
    return Results.Ok("ChatBridge Chat API is working!");
});

app.Run();
