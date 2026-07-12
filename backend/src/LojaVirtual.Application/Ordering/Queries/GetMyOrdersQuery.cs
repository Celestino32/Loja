using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Ordering.Queries;

public record GetMyOrdersQuery(Guid UserId) : IRequest<List<OrderSummaryDto>>;

public class GetMyOrdersQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyOrdersQuery, List<OrderSummaryDto>>
{
    public async Task<List<OrderSummaryDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (customer is null)
            return [];

        return await context.Orders.AsNoTracking()
            .Where(o => o.CustomerId == customer.Id)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new OrderSummaryDto(o.Id, o.OrderNumber, o.Status, o.Total, o.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
