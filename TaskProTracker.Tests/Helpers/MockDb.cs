using Microsoft.EntityFrameworkCore;
using TaskProTracker.MinimalAPI.Data;

namespace TaskProTracker.Tests.Helpers
{
    public class MockDb : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

            return new AppDbContext(options);
        }
    }
}
