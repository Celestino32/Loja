using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Payments;

/// <summary>
/// Registo de pagamento de um pedido. Para Multicaixa Express, "ExternalReference" guarda o
/// identificador da cobrança no gateway; para Referência de Pagamento, o próprio código gerado;
/// para Transferência Bancária, fica nulo até confirmação manual pelo financeiro.
/// </summary>
public class Payment : AuditableEntity
{
    public Guid OrderId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pendente;
    public decimal Amount { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Instructions { get; private set; }
    public DateTime? ConfirmedAtUtc { get; private set; }

    private Payment()
    {
    }

    public Payment(Guid orderId, PaymentMethod method, decimal amount, string? externalReference, string? instructions)
    {
        if (amount <= 0)
            throw new DomainException("O valor do pagamento deve ser maior que zero.");

        OrderId = orderId;
        Method = method;
        Amount = amount;
        ExternalReference = externalReference;
        Instructions = instructions;
    }

    public void Confirm(string? externalReference = null)
    {
        if (Status == PaymentStatus.Confirmado)
            throw new DomainException("O pagamento já está confirmado.");
        if (Status == PaymentStatus.Falhado)
            throw new DomainException("Não é possível confirmar um pagamento que já falhou.");

        if (!string.IsNullOrWhiteSpace(externalReference))
            ExternalReference = externalReference;

        Status = PaymentStatus.Confirmado;
        ConfirmedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Fail()
    {
        if (Status == PaymentStatus.Confirmado)
            throw new DomainException("Não é possível falhar um pagamento já confirmado.");

        Status = PaymentStatus.Falhado;
        Touch();
    }
}
