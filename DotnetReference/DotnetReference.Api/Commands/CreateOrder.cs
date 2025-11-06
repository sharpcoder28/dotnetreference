using DotnetReference.Api.Domain;
using DotnetReference.Api.Domain.Services;

namespace DotnetReference.Api.Commands;

public record CreateOrder(
    DateTime OrderDate,
    List<CreateOrderItem> Items
)
{
    public Order ToOrder(ITaxCalculator taxCalculator)
    {
        var orderItems = Items.Select(item => new OrderItem(
            item.ProductName,
            item.Quantity,
            item.UnitPrice
        )).ToList();

        return new Order(orderItems, taxCalculator);
    }
}

public record CreateOrderItem(
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
