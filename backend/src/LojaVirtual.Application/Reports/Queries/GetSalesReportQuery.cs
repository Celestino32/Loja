using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Ordering;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Reports.Queries;

/// <summary>
/// Conta como "venda" qualquer pedido que passou de Pendente (ou seja, o pagamento foi
/// confirmado), exceto os cancelados — pedidos nunca pagos não entram nas receitas.
/// </summary>
public record GetSalesReportQuery(DateOnly From, DateOnly To) : IRequest<SalesReportDto>;

public class GetSalesReportQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSalesReportQuery, SalesReportDto>
{
    public async Task<SalesReportDto> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var fromUtc = request.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toUtc = request.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var orders = await context.Orders.AsNoTracking()
            .Where(o => o.Status != OrderStatus.Pendente && o.Status != OrderStatus.Cancelado)
            .Where(o => o.CreatedAtUtc >= fromUtc && o.CreatedAtUtc <= toUtc)
            .Select(o => new { o.CreatedAtUtc, o.Total })
            .ToListAsync(cancellationToken);

        var dailyBreakdown = orders
            .GroupBy(o => DateOnly.FromDateTime(o.CreatedAtUtc))
            .Select(g => new DailySalesDto(g.Key, g.Sum(o => o.Total), g.Count()))
            .OrderBy(d => d.Date)
            .ToList();

        var totalRevenue = orders.Sum(o => o.Total);
        var orderCount = orders.Count;
        var averageOrderValue = orderCount == 0 ? 0 : Math.Round(totalRevenue / orderCount, 2);

        return new SalesReportDto(totalRevenue, orderCount, averageOrderValue, dailyBreakdown);
    }
}
