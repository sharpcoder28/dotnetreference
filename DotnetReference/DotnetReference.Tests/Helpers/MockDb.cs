using DotnetReference.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace DotnetReference.Tests.Helpers;

public class MockDb : IDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new OrderDbContext(options);
    }
}
