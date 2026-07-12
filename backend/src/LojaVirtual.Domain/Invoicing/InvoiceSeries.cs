using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Invoicing;

/// <summary>
/// Série de faturação (ex: "FT 2026"). Garante numeração sequencial e sem furos,
/// requisito do regime jurídico das faturas em Angola para futura certificação AGT.
/// </summary>
public class InvoiceSeries : BaseEntity
{
    public string Code { get; private set; } = default!;
    public int LastSequenceNumber { get; private set; }

    private InvoiceSeries()
    {
    }

    public InvoiceSeries(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("O código da série de faturação é obrigatório.");
        Code = code.Trim();
        LastSequenceNumber = 0;
    }

    /// <summary>
    /// Reserva o próximo número sequencial. Deve ser chamado dentro de uma transação
    /// com bloqueio apropriado para evitar números duplicados sob concorrência.
    /// </summary>
    public int ReserveNextSequenceNumber()
    {
        LastSequenceNumber++;
        return LastSequenceNumber;
    }
}
