namespace Warehouse.Application.Queries.Products.GroupProductsByExpiryYearAndCountry;

public class GroupProductsByExpiryYearAndCountryResponse
{
    public int ExpiryYear { get; set; }

    public string Country { get; set; } = string.Empty;

    public int Count { get; set; }
}