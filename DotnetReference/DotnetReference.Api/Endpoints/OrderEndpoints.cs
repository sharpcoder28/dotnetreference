using DotnetReference.Api.Commands;
using DotnetReference.Api.Data;
using DotnetReference.Api.Domain;
using DotnetReference.Api.Domain.Services;
using DotnetReference.Api.Dto;
using DotnetReference.Api.Queries;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetReference.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder");

        group.MapGet("/{orderId}", GetOrderById)
            .WithName("GetOrderById");

        group.MapGet("/customer/{customerId}", GetOrdersByCustomer)
            .WithName("GetOrdersByCustomer");
    }

    public static async Task<Created<OrderResponse>> CreateOrder(
        CreateOrder command, 
        OrderDbContext dbContext, 
        ITaxCalculator taxCalculator)
    {
        var order = command.ToOrder(taxCalculator);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var response = new OrderResponse(order);

        return TypedResults.Created($"/api/orders/{order.Id}", response);
    }

    public static async Task<Results<Ok<OrderResponse>, NotFound>> GetOrderById(
        Guid orderId,
        IOrderQueries orderQueries)
    {
        var order = await orderQueries.GetOrderById(orderId);

        return order is not null
            ? TypedResults.Ok(order)
            : TypedResults.NotFound();
    }
    
    public static async Task<Ok<OrderListResponse[]>> GetOrdersByCustomer(
        Guid customerId, 
        IOrderQueries orderQueries)
    {
        var orders = await orderQueries.GetOrdersByCustomer(customerId);

        return TypedResults.Ok(orders);
    }
}
