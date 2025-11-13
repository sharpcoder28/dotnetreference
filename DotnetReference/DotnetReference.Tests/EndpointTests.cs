using DotnetReference.Api.Commands;
using DotnetReference.Api.Domain;
using DotnetReference.Api.Domain.Services;
using DotnetReference.Api.Dto;
using DotnetReference.Api.Endpoints;
using DotnetReference.Api.Queries;
using DotnetReference.Tests.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace DotnetReference.Tests;

public class EndpointTests
{

    [Fact]
    public async Task CreateOrder_ValidRequest_ReturnsCreated()
    {
        // Arrange
        await using var context = new MockDb().CreateDbContext();
        
        var mockTaxCalculator = new Mock<ITaxCalculator>();
        mockTaxCalculator.Setup(m => m.CalculateTax(It.IsAny<decimal>()))
            .Returns((decimal amount) => amount * 0.10m); // 10% tax
        
        var customerId = Guid.NewGuid();
        var command = new CreateOrder(
            customerId,
            DateTime.UtcNow,
            [
                new Api.Commands.OrderItem("Product 1", 2, 10.00m),
                new Api.Commands.OrderItem("Product 2", 1, 20.00m)
            ]
        );

        // Act
        var result = await OrderEndpoints.CreateOrder(command, context, mockTaxCalculator.Object);

        // Assert
        Assert.IsType<Created<OrderResponse>>(result);
        Assert.NotNull(result.Value);
        
        var order = result.Value;
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(40.00m, order.NetAmount);
        Assert.Equal(4.00m, order.TaxAmount);
        Assert.Equal(44.00m, order.TotalAmount);
        
        // Verify tax calculator was called with the correct amount
        mockTaxCalculator.Verify(m => m.CalculateTax(40.00m), Times.Once);
    }

    [Fact]
    public async Task GetOrdersByCustomer_ExistingCustomer_ReturnsOrders()
    {
        // Arrange
        await using var context = new MockDb().CreateDbContext();
        
        var mockTaxCalculator = new Mock<ITaxCalculator>();
        mockTaxCalculator.Setup(m => m.CalculateTax(It.IsAny<decimal>()))
            .Returns((decimal amount) => amount * 0.10m);
        
        var customerId = Guid.NewGuid();
        
        var order1 = new Order(
            customerId,
            [new Api.Domain.OrderItem("Product A", 1, 10.00m)],
            mockTaxCalculator.Object
        );
        var order2 = new Order(
            customerId,
            [new Api.Domain.OrderItem("Product B", 2, 15.00m)],
            mockTaxCalculator.Object
        );

        context.Orders.Add(order1);
        context.Orders.Add(order2);
        await context.SaveChangesAsync();

        var orderQueries = new OrderQueries(context);

        // Act
        var result = await OrderEndpoints.GetOrdersByCustomer(customerId, orderQueries);

        // Assert
        Assert.IsType<Ok<OrderListResponse[]>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Length);
        Assert.All(result.Value, order => Assert.NotEqual(Guid.Empty, order.OrderId));
    }

    [Fact]
    public async Task GetOrdersByCustomer_NonExistingCustomer_ReturnsEmptyList()
    {
        // Arrange
        await using var context = new MockDb().CreateDbContext();
        
        var nonExistentCustomerId = Guid.NewGuid();
        var orderQueries = new OrderQueries(context);

        // Act
        var result = await OrderEndpoints.GetOrdersByCustomer(nonExistentCustomerId, orderQueries);

        // Assert
        Assert.IsType<Ok<OrderListResponse[]>>(result);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetOrdersByCustomer_MultipleCustomers_ReturnsOnlyMatchingOrders()
    {
        // Arrange
        await using var context = new MockDb().CreateDbContext();
        
        var mockTaxCalculator = new Mock<ITaxCalculator>();
        mockTaxCalculator.Setup(m => m.CalculateTax(It.IsAny<decimal>()))
            .Returns((decimal amount) => amount * 0.10m);
        
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();

        var order1 = new Order(
            customer1Id,
            [new Api.Domain.OrderItem("Product 1", 1, 10.00m)],
            mockTaxCalculator.Object
        );
        var order2 = new Order(
            customer2Id,
            [new Api.Domain.OrderItem("Product 2", 1, 20.00m)],
            mockTaxCalculator.Object
        );
        var order3 = new Order(
            customer1Id,
            [new Api.Domain.OrderItem("Product 3", 1, 30.00m)],
            mockTaxCalculator.Object
        );

        context.Orders.AddRange(order1, order2, order3);
        await context.SaveChangesAsync();

        var orderQueries = new OrderQueries(context);

        // Act
        var result = await OrderEndpoints.GetOrdersByCustomer(customer1Id, orderQueries);

        // Assert
        Assert.IsType<Ok<OrderListResponse[]>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Length);
        Assert.All(result.Value, order => Assert.NotEqual(Guid.Empty, order.OrderId));
    }

    [Fact]
    public async Task CreateOrder_WithMultipleItems_CalculatesTotalCorrectly()
    {
        // Arrange
        await using var context = new MockDb().CreateDbContext();
        
        var mockTaxCalculator = new Mock<ITaxCalculator>();
        mockTaxCalculator.Setup(m => m.CalculateTax(It.IsAny<decimal>()))
            .Returns((decimal amount) => amount * 0.10m);
        
        var customerId = Guid.NewGuid();
        var command = new CreateOrder(
            customerId,
            DateTime.UtcNow,
            [
                new Api.Commands.OrderItem("Product 1", 3, 15.00m),
                new Api.Commands.OrderItem("Product 2", 2, 25.50m),
                new Api.Commands.OrderItem("Product 3", 1, 100.00m)
            ]
        );

        // Act
        var result = await OrderEndpoints.CreateOrder(command, context, mockTaxCalculator.Object);

        // Assert
        Assert.IsType<Created<OrderResponse>>(result);
        Assert.NotNull(result.Value);
        
        var order = result.Value;
        Assert.Equal(196.00m, order.NetAmount); // (3*15) + (2*25.5) + (1*100) = 45 + 51 + 100
        Assert.Equal(19.60m, order.TaxAmount);
        Assert.Equal(215.60m, order.TotalAmount);
        
        // Verify tax calculator was called with the net amount
        mockTaxCalculator.Verify(m => m.CalculateTax(196.00m), Times.Once);
    }
}
