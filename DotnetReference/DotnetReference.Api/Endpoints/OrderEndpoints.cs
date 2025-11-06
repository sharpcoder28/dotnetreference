using DotnetReference.Api.Commands;
using DotnetReference.Api.Data;
using DotnetReference.Api.Domain;
using DotnetReference.Api.Domain.Services;

namespace DotnetReference.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapPost("/", async (CreateOrder command, OrderDbContext dbContext, ITaxCalculator taxCalculator) =>
        {
            var order = command.ToOrder(taxCalculator);

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/orders/{order.Id}", order);
        })
        .WithName("CreateOrder");
    }
}
