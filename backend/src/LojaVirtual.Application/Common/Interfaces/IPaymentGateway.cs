using LojaVirtual.Domain.Payments;

namespace LojaVirtual.Application.Common.Interfaces;

public record PaymentInitiationResult(string? ExternalReference, string Instructions);

/// <summary>
/// Abstração para os diferentes meios de pagamento aceites pela loja. Cada implementação
/// cobre um <see cref="PaymentMethod"/>; o handler de checkout escolhe a implementação certa
/// via <see cref="Method"/> e não precisa conhecer os detalhes de cada gateway.
/// </summary>
public interface IPaymentGateway
{
    PaymentMethod Method { get; }

    Task<PaymentInitiationResult> InitiateAsync(Guid orderId, string orderNumber, decimal amount, CancellationToken cancellationToken);
}
