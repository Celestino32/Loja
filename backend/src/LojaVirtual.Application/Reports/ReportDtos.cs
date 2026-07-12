namespace LojaVirtual.Application.Reports;

public record DailySalesDto(DateOnly Date, decimal Revenue, int OrderCount);

public record SalesReportDto(decimal TotalRevenue, int OrderCount, decimal AverageOrderValue, List<DailySalesDto> DailyBreakdown);

public record TopProductDto(Guid ProductId, string ProductName, int QuantitySold, decimal Revenue);

public record LowStockProductDto(Guid ProductId, string ProductName, string Sku, int StockQuantity);
