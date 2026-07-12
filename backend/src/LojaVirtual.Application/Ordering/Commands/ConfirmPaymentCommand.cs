using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Application.Invoicing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Ordering.Commands;

/// <summary>
/// Confirma o pagamento de um pedido — manualmente pelo financeiro (Referência/Transferência)
/// ou via webhook do facilitador (Multicaixa Express) — e emite a fatura automaticamente.
/// </summary>
public record ConfirmPaymentCommand(Guid OrderId, string? ExternalReference = null) : IRequest;

public class ConfirmPaymentCommandHandler(IApplicationDbContext context, IInvoiceIssuer invoiceIssuer)
    : IRequestHandler<ConfirmPaymentCommand>
{
    public async Task Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.Payment)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Ordering.Order), request.OrderId);

        var payment = order.Payment
            ?? throw new NotFoundException(nameof(Domain.Payments.Payment), request.OrderId);

        payment.Confirm(request.ExternalReference);
        order.MarkAsPaid();

        await context.SaveChangesAsync(cancellationToken);
        await invoiceIssuer.IssueForOrderAsync(order, cancellationToken);
    }
}
