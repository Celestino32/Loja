using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Reviews.Queries;

public record GetProductReviewsQuery(Guid ProductId) : IRequest<ProductReviewsDto>;

public class GetProductReviewsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductReviewsQuery, ProductReviewsDto>
{
    public async Task<ProductReviewsDto> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await context.Reviews.AsNoTracking()
            .Where(r => r.ProductId == request.ProductId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new ReviewDto(r.Id, r.CustomerName, r.Rating, r.Comment, r.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        var average = reviews.Count == 0 ? 0 : Math.Round(reviews.Average(r => r.Rating), 1);
        return new ProductReviewsDto(average, reviews.Count, reviews);
    }
}
