using DotnetReference.Api.Domain.Services;

namespace DotnetReference.Api.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }

    public List<OrderItem> Items { get; private set; } = [];
    public decimal TaxAmount { get; private set; }
    public decimal Subtotal => Items.Sum(x => x.TotalPrice);
    public decimal Total => Subtotal + TaxAmount;

    // Empty constructor for EF
    private Order() { }

    public Order(Guid customerId, List<OrderItem> items, ITaxCalculator taxCalculator)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Items = items;
        OrderDate = DateTime.UtcNow;

        TaxAmount = taxCalculator.CalculateTax(Subtotal);
    }
}