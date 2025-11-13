using DotnetReference.Api.Domain;
using DotnetReference.Api.Domain.Services;

namespace DotnetReference.Api.Commands;

public record CreateOrder(
    Guid CustomerId,
    DateTime OrderDate,
    List<OrderItem> Items
)
{
    public Order ToOrder(ITaxCalculator taxCalculator)
    {
        var orderItems = Items.Select(item => new Domain.OrderItem(
            item.ProductName,
            item.Quantity,
            item.UnitPrice
        )).ToList();

        return new Order(CustomerId, orderItems, taxCalculator);
    }
}

public record OrderItem(
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
