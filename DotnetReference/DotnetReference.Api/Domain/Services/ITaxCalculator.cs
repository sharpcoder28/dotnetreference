namespace DotnetReference.Api.Domain.Services;

public interface ITaxCalculator
{
    decimal CalculateTax(decimal amount);
}

public class WebTaxCalculator : ITaxCalculator
{
    public WebTaxCalculator()
    {
        // TODO: HttpClient injection for real tax service calls
    }

    public decimal CalculateTax(decimal amount)
    {
        // Simple tax calculation logic (e.g., 10% tax rate)
        return amount * 0.10m;
    }
}
