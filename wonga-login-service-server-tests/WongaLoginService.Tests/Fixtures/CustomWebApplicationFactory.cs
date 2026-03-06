using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using WongaLoginService.Data;

namespace WongaLoginService.Tests.Fixtures;

/// <summary>
/// Uses Testcontainers for PostgreSQL integration testing.
/// Implements IAsyncLifetime to properly manage container lifecycle.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;

    public CustomWebApplicationFactory()
    {
        // Configure PostgreSQL container
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("wonga_test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true) // Auto-cleanup after disposal
            .Build();
    }

    /// <summary>
    /// Initialize and start PostgreSQL container before any tests run.
    /// </summary>
    public async Task InitializeAsync()
    {
        // Start container and wait for it to be ready
        await _postgresContainer.StartAsync();

        // Verify container is healthy
        var result = await _postgresContainer.ExecAsync(new[]
        {
            "pg_isready", "-U", "test_user", "-d", "wonga_test_db"
        });

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException("PostgreSQL container failed health check");
        }
    }

    /// <summary>
    /// Clean up and dispose of PostgreSQL container after all tests complete.
    /// </summary>
    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    /// <summary>
    /// Configure test application to use the Testcontainers PostgreSQL instance
    /// and override JWT settings for predictable test tokens.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Override connection string to use Testcontainers PostgreSQL
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgresContainer.GetConnectionString(),
                
                // Override JWT settings for predictable testing
                ["JWT:Key"] = "TestSecretKeyForJwtTokenGeneration123456789",
                ["JWT:Issuer"] = "WongaTestIssuer",
                ["JWT:Audience"] = "WongaTestAudience",
                ["JWT:ExpiryMinutes"] = "60"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing AppDbContext registration
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            // Register AppDbContext with Testcontainers connection string
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });

            // Build service provider for migrations
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Apply migrations to test DB
            dbContext.Database.Migrate();
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Helper method to get scoped DbContext for test setup/teardown operations.
    /// </summary>
    public AppDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// Helper method to reset DB.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Delete all data but keep schema
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE users RESTART IDENTITY CASCADE");
    }
}
