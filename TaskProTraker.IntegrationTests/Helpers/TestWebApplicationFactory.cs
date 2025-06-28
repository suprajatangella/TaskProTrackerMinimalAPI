using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskProTracker.MinimalAPI.Data;

namespace TaskProTraker.IntegrationTests.Helpers
{
    public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.IntegrationTest.json")
                        .Build();

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
                });
            });

            return base.CreateHost(builder);
        }
    }
}
