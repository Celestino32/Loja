using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Reviews.Queries;

/// <summary>Produtos já avaliados pelo cliente dentro de um pedido específico — usado para
/// esconder o botão "Avaliar" nos itens já avaliados.</summary>
public record GetMyReviewedProductsQuery(Guid UserId, Guid OrderId) : IRequest<List<Guid>>;

public class GetMyReviewedProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyReviewedProductsQuery, List<Guid>>
{
    public async Task<List<Guid>> Handle(GetMyReviewedProductsQuery request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);
        if (customer is null) return [];

        return await context.Reviews.AsNoTracking()
            .Where(r => r.OrderId == request.OrderId && r.CustomerId == customer.Id)
            .Select(r => r.ProductId)
            .ToListAsync(cancellationToken);
    }
}
