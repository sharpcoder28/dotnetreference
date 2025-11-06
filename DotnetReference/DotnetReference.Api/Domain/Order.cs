using DotnetReference.Api.Domain.Services;

namespace DotnetReference.Api.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public DateTime OrderDate { get; private set; }

    public List<OrderItem> Items { get; private set; } = [];
    public decimal TaxAmount { get; private set; }
    public decimal NetAmount => Items.Sum(x => x.TotalPrice);
    public decimal TotalAmount => NetAmount + TaxAmount;

    public Order(List<OrderItem> items, ITaxCalculator taxCalculator)
    {
        Id = Guid.NewGuid();
        Items = items;
        OrderDate = DateTime.UtcNow;

        TaxAmount = taxCalculator.CalculateTax(NetAmount);
    }
}