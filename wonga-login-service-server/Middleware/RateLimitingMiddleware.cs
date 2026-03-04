using System.Collections.Concurrent;

namespace WongaLoginService.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, (DateTime, int)> _requests = new();
    private const int MaxAttempts = 5;
    private static readonly TimeSpan TimeWindow = TimeSpan.FromMinutes(15);

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.Request.Path.Value;
        
        // Only rate limit auth endpoints
        if (endpoint?.StartsWith("/api/auth/") == true)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"{ipAddress}:{endpoint}";

            CleanupOldEntries();

            if (_requests.TryGetValue(key, out var entry))
            {
                var (firstAttempt, attempts) = entry;
                
                if (DateTime.UtcNow - firstAttempt < TimeWindow)
                {
                    if (attempts >= MaxAttempts)
                    {
                        context.Response.StatusCode = 429;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Too many attempts. Please try again later."
                        });
                        return;
                    }
                    
                    _requests[key] = (firstAttempt, attempts + 1);
                }
                else
                {
                    _requests[key] = (DateTime.UtcNow, 1);
                }
            }
            else
            {
                _requests[key] = (DateTime.UtcNow, 1);
            }
        }

        await _next(context);
    }

    private static void CleanupOldEntries()
    {
        var cutoff = DateTime.UtcNow - TimeWindow;
        var keysToRemove = _requests
            .Where(kvp => kvp.Value.Item1 < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _requests.TryRemove(key, out _);
        }
    }
}
