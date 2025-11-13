using DotnetReference.Api.Domain;
using DotnetReference.Api.Domain.Services;

namespace DotnetReference.Tests;

public class DomainTests
{
    private class MockTaxCalculator : ITaxCalculator
    {
        private readonly decimal _taxRate;

        public MockTaxCalculator(decimal taxRate = 0.10m)
        {
            _taxRate = taxRate;
        }

        public decimal CalculateTax(decimal amount)
        {
            return amount * _taxRate;
        }
    }

    [Fact]
    public void OrderItem_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var productName = "Test Product";
        var quantity = 5;
        var unitPrice = 10.50m;

        // Act
        var orderItem = new OrderItem(productName, quantity, unitPrice);

        // Assert
        Assert.NotEqual(Guid.Empty, orderItem.Id);
        Assert.Equal(productName, orderItem.ProductName);
        Assert.Equal(quantity, orderItem.Quantity);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
    }

    [Fact]
    public void OrderItem_TotalPrice_CalculatedCorrectly()
    {
        // Arrange
        var quantity = 3;
        var unitPrice = 25.00m;
        var orderItem = new OrderItem("Product", quantity, unitPrice);

        // Act
        var totalPrice = orderItem.TotalPrice;

        // Assert
        Assert.Equal(75.00m, totalPrice);
    }

    [Fact]
    public void Order_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem("Product 1", 2, 10.00m),
            new OrderItem("Product 2", 1, 20.00m)
        };
        var taxCalculator = new MockTaxCalculator(0.10m);

        // Act
        var order = new Order(customerId, items, taxCalculator);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(items.Count, order.Items.Count);
        Assert.True(order.OrderDate <= DateTime.UtcNow);
        Assert.True(order.OrderDate >= DateTime.UtcNow.AddSeconds(-5));
    }

    [Fact]
    public void Order_NetAmount_CalculatedCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem("Product 1", 2, 10.00m), // 20.00
            new OrderItem("Product 2", 1, 30.00m)  // 30.00
        };
        var taxCalculator = new MockTaxCalculator();

        // Act
        var order = new Order(customerId, items, taxCalculator);

        // Assert
        Assert.Equal(50.00m, order.Subtotal);
    }

    [Fact]
    public void Order_TaxAmount_CalculatedCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem("Product 1", 2, 10.00m)
        };
        var taxCalculator = new MockTaxCalculator(0.15m);

        // Act
        var order = new Order(customerId, items, taxCalculator);

        // Assert
        Assert.Equal(3.00m, order.TaxAmount); // 20.00 * 0.15
    }

    [Fact]
    public void Order_TotalAmount_CalculatedCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem("Product 1", 5, 10.00m) // 50.00
        };
        var taxCalculator = new MockTaxCalculator(0.10m);

        // Act
        var order = new Order(customerId, items, taxCalculator);

        // Assert
        Assert.Equal(50.00m, order.Subtotal);
        Assert.Equal(5.00m, order.TaxAmount);
        Assert.Equal(55.00m, order.Total);
    }

    [Fact]
    public void Order_EmptyItems_NetAmountIsZero()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>();
        var taxCalculator = new MockTaxCalculator();

        // Act
        var order = new Order(customerId, items, taxCalculator);

        // Assert
        Assert.Equal(0m, order.Subtotal);
        Assert.Equal(0m, order.TaxAmount);
        Assert.Equal(0m, order.Total);
    }
}
