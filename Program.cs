using LoggingAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using LoggingAPI.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Global Exception Handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;

        // Log via ILogger (goes to configured logging providers)
        var logger = errorApp.ApplicationServices.GetRequiredService<ILogger<Program>>();
        if (exception is not null)
        {
            logger.LogError(exception, "Unhandled exception occurred while processing request.");

            // Persist to DB using a scoped AppDbContext
            try
            {
                var scopeFactory = errorApp.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var sysLog = new SystemLog
                {
                    LogLevel = "Error",
                    Message = exception.ToString(),
                    Timestamp = DateTime.UtcNow
                };

                db.SystemLogs.Add(sysLog);
                await db.SaveChangesAsync();
            }
            catch (Exception dbEx)
            {
                logger.LogWarning(dbEx, "Failed to persist exception to database.");
            }
        }

        // Additionally attempt to write to Windows Event Log when running on Windows.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                const string source = "LoggingAPI";
                const string logName = "Application";

                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, logName);
                }

                EventLog.WriteEntry(source, exception?.ToString() ?? "An unexpected system error occurred.", EventLogEntryType.Error);
            }
            catch
            {
                // Swallow any event-log related errors to avoid failing the request pipeline
            }
        }

        await context.Response.WriteAsJsonAsync(new
        {
            StatusCode = 500,
            Message = "An unexpected system error occurred."
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
