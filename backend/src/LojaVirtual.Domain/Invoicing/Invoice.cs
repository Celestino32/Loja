using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Invoicing;

/// <summary>
/// Fatura fiscal-ready: imutável após emissão, numeração sequencial por série, IVA discriminado.
/// A emissão de "segunda via" reutiliza estes mesmos dados para gerar novamente o PDF —
/// nunca cria um novo número de fatura. A certificação formal do software junto à AGT
/// (SAF-T AO, assinatura digital) é um passo administrativo posterior, fora deste domínio.
/// </summary>
public class Invoice : AuditableEntity
{
    public const decimal AngolaVatRate = 0.14m;

    public Guid InvoiceSeriesId { get; private set; }
    public string SeriesCode { get; private set; } = default!;
    public int SequenceNumber { get; private set; }
    public string FullNumber => $"{SeriesCode}/{SequenceNumber:D5}";

    public Guid OrderId { get; private set; }

    public string SellerName { get; private set; } = default!;
    public string SellerNif { get; private set; } = default!;

    public string CustomerName { get; private set; } = default!;
    public string? CustomerNif { get; private set; }

    public decimal Subtotal { get; private set; }
    public decimal VatRate { get; private set; } = AngolaVatRate;
    public decimal VatAmount { get; private set; }
    public decimal Total { get; private set; }

    public DateTime IssuedAtUtc { get; private set; }
    public bool IsCancelled { get; private set; }

    private Invoice()
    {
    }

    public Invoice(
        Guid invoiceSeriesId,
        string seriesCode,
        int sequenceNumber,
        Guid orderId,
        string sellerName,
        string sellerNif,
        string customerName,
        string? customerNif,
        decimal subtotal)
    {
        if (string.IsNullOrWhiteSpace(seriesCode))
            throw new DomainException("O código da série é obrigatório.");
        if (sequenceNumber <= 0)
            throw new DomainException("O número sequencial da fatura deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(sellerName) || string.IsNullOrWhiteSpace(sellerNif))
            throw new DomainException("Os dados fiscais do vendedor (nome e NIF) são obrigatórios.");
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("O nome do cliente é obrigatório na fatura.");
        if (subtotal < 0)
            throw new DomainException("O subtotal da fatura não pode ser negativo.");

        InvoiceSeriesId = invoiceSeriesId;
        SeriesCode = seriesCode.Trim();
        SequenceNumber = sequenceNumber;
        OrderId = orderId;
        SellerName = sellerName.Trim();
        SellerNif = sellerNif.Trim();
        CustomerName = customerName.Trim();
        CustomerNif = customerNif?.Trim();
        Subtotal = subtotal;
        VatAmount = Math.Round(subtotal * AngolaVatRate, 2, MidpointRounding.AwayFromZero);
        Total = Subtotal + VatAmount;
        IssuedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Anula fiscalmente a fatura (não apaga o registo — mantém rasto para auditoria/AGT).
    /// A emissão de nota de crédito correspondente é tratada em fase posterior.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            throw new DomainException("A fatura já está anulada.");
        IsCancelled = true;
        Touch();
    }
}
