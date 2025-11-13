using DotnetReference.Api.Data;
using DotnetReference.Api.Dto;
using Microsoft.EntityFrameworkCore;

namespace DotnetReference.Api.Queries;

public class OrderQueries(OrderDbContext db) : IOrderQueries
{
    public async Task<OrderListResponse[]> GetOrdersByCustomer(Guid customerId)
    {
        var orders = await db.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .Select(o => new OrderListResponse(
                o.Id,
                o.Total,
                o.OrderDate,
                string.Join(", ", o.Items.Select(x => x.ProductName))
            ))
            .ToArrayAsync();

        return orders;
    }

    public async Task<OrderResponse?> GetOrderById(Guid orderId)
    {
        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        return order is not null ? new OrderResponse(order) : null;
    }
}