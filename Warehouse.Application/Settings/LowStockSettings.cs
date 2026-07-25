namespace Warehouse.Application.Settings;

public class LowStockSettings
{
    public const string SectionName = "LowStock";

    public int Threshold { get; set; }
}