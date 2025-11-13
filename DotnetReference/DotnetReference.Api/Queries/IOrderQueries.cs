using DotnetReference.Api.Dto;

namespace DotnetReference.Api.Queries;

public interface IOrderQueries
{
    Task<OrderListResponse[]> GetOrdersByCustomer(Guid customerId);
    Task<OrderResponse?> GetOrderById(Guid orderId);
}
