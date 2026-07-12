using FluentValidation;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Identity;
using LojaVirtual.Domain.Ordering;
using LojaVirtual.Domain.Reviews;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Reviews.Commands;

public record CreateReviewCommand(Guid UserId, Guid ProductId, Guid OrderId, int Rating, string? Comment) : IRequest<Guid>;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(2000);
    }
}

public class CreateReviewCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateReviewCommand, Guid>
{
    public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), request.UserId);

        var order = await context.Orders.AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if (order.CustomerId != customer.Id)
            throw new NotFoundException(nameof(Order), request.OrderId);

        if (order.Status != OrderStatus.Concluido)
            throw new DomainException("Só é possível avaliar produtos de pedidos concluídos.");

        if (order.Items.All(i => i.ProductId != request.ProductId))
            throw new DomainException("Este produto não faz parte do pedido informado.");

        var alreadyReviewed = await context.Reviews.AsNoTracking()
            .AnyAsync(r => r.OrderId == request.OrderId && r.ProductId == request.ProductId, cancellationToken);
        if (alreadyReviewed)
            throw new DomainException("Você já avaliou este produto neste pedido.");

        var review = new Review(request.ProductId, customer.Id, request.OrderId, request.Rating, request.Comment, customer.FullName);
        context.Reviews.Add(review);
        await context.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}
