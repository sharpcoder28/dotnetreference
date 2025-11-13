using DotnetReference.Api.Domain;

namespace DotnetReference.Api.Dto;

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    DateTime OrderDate,
    List<OrderItemResponse> Items,
    decimal NetAmount,
    decimal TaxAmount,
    decimal TotalAmount
)
{
    public OrderResponse(Order order) : this(
        order.Id,
        order.CustomerId,
        order.OrderDate,
        [.. order.Items.Select(i => new OrderItemResponse(
            i.Id,
            i.ProductName,
            i.Quantity,
            i.UnitPrice,
            i.TotalPrice
        ))],
        order.Subtotal,
        order.TaxAmount,
        order.Total
    )
    { }
}
