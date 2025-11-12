using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using efexample.Data;
using Microsoft.Extensions.Configuration;

namespace efexample.Tests;

public class TestFixture : IDisposable
{
    public SchoolContext Context { get; private set; }
    private readonly DbContextOptions<SchoolContext> _options;
    private readonly bool _useInMemoryDb;
    private bool _disposed = false;
    
    // Configuration key for the connection string when using real database
    private const string ConnectionStringKey = "DefaultConnection";

    public TestFixture()
    {
        // Set this to false to use a real database
        // You can also move this to appsettings.json or environment variables
        _useInMemoryDb = false;

        var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();

        if (_useInMemoryDb)
        {
            // Use in-memory database for testing
            var databaseName = $"SchoolDb_Test_{Guid.NewGuid()}";
            optionsBuilder.UseInMemoryDatabase(databaseName);
            
            _options = optionsBuilder.Options;
            Context = new SchoolContext(_options);
            Context.Database.EnsureCreated();
        }
        else
        {
            // Use real database for testing
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();
                
            var connectionString = configuration.GetConnectionString(ConnectionStringKey);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{ConnectionStringKey}' not found in configuration.");
            }
            
            optionsBuilder.UseNpgsql(connectionString);
            _options = optionsBuilder.Options;
            Context = new SchoolContext(_options);
            
            // For real database, you might want to run migrations or create the database
            // Context.Database.EnsureCreated();
            // Or use migrations:
            // Context.Database.Migrate();
        }
    }

    public void ResetDatabase()
    {
        if (_useInMemoryDb)
        {
            // Clear all data from the in-memory database
            Context.Students.RemoveRange(Context.Students);
            Context.Courses.RemoveRange(Context.Courses);
            Context.StudentCourses.RemoveRange(Context.StudentCourses);
            Context.SaveChanges();
        }
        else
        {
            // For real database, you might want to be more careful with cleanup
            // This is a basic example - you might want to use transactions or a different approach
            using var transaction = Context.Database.BeginTransaction();
            try
            {
                // Use PostgreSQL specific SQL syntax for deletion
                Context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"StudentCourses\" CASCADE");
                Context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Students\" CASCADE");
                Context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Courses\" CASCADE");
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_useInMemoryDb)
                {
#if CLEANUP_DATABASE
                    // Clean up the in-memory database
                    Context.Database.EnsureDeleted();
#endif
                }
                else
                {
                    // For real database, you might want to clean up test data
                    // or leave it for inspection after tests
                }
                Context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
