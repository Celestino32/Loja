using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Shipping;

/// <summary>
/// Zona de entrega por província/município, com custo fixo. Usada no checkout para
/// calcular o frete quando o cliente escolhe entrega (em vez de retirada na loja).
/// </summary>
public class ShippingZone : AuditableEntity
{
    public string Province { get; private set; } = default!;
    public string Municipality { get; private set; } = default!;
    public decimal Cost { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ShippingZone()
    {
    }

    public ShippingZone(string province, string municipality, decimal cost)
    {
        SetLocation(province, municipality);
        SetCost(cost);
    }

    public void Update(string province, string municipality, decimal cost)
    {
        SetLocation(province, municipality);
        SetCost(cost);
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    private void SetLocation(string province, string municipality)
    {
        if (string.IsNullOrWhiteSpace(province))
            throw new DomainException("A província é obrigatória.");
        if (string.IsNullOrWhiteSpace(municipality))
            throw new DomainException("O município é obrigatório.");
        Province = province.Trim();
        Municipality = municipality.Trim();
    }

    private void SetCost(decimal cost)
    {
        if (cost < 0)
            throw new DomainException("O custo de entrega não pode ser negativo.");
        Cost = cost;
    }
}
