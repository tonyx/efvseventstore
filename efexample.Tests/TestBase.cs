using Microsoft.Extensions.DependencyInjection;
using efexample.Data;
using efexample.Application.Interfaces;
using efexample.Application.Services;

namespace efexample.Tests
{
    public abstract class TestBase
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly SchoolContext DbContext;
        protected readonly TestFixture _fixture;

        protected TestBase(TestFixture fixture)
        {
            _fixture = fixture;
            var services = new ServiceCollection();
            
            // Register services
            services.AddScoped<ISchoolService, SchoolService>();
            services.AddScoped(_ => fixture.Context);
            
            ServiceProvider = services.BuildServiceProvider();
            DbContext = fixture.Context;
        }

        protected T GetService<T>() where T : class
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
