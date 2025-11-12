using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using efexample.Data;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Create host builder
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add DbContext
        services.AddDbContext<SchoolContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                }));

        // Add other services here if needed
    })
    .Build();

// Create a scope to resolve services
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    // Get the database context
    var context = services.GetRequiredService<SchoolContext>();
    
    // Apply any pending migrations
    context.Database.Migrate();
    
    Console.WriteLine("Database migration completed successfully!");
    
    // Your application logic here
    Console.WriteLine("Application started successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
}

// Keep the application running
await host.RunAsync();
