namespace DotnetReference.Api.Dto;

public record OrderListResponse(
    Guid OrderId,
    decimal OrderTotal,
    DateTime OrderDate,
    string ItemDescriptions);
