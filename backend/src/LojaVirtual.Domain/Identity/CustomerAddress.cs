using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Identity;

public class CustomerAddress : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public string Province { get; private set; } = default!;
    public string Municipality { get; private set; } = default!;
    public string Street { get; private set; } = default!;
    public string? Reference { get; private set; }
    public bool IsDefault { get; private set; }

    private CustomerAddress()
    {
    }

    public CustomerAddress(Guid customerId, string province, string municipality, string street, string? reference, bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(province))
            throw new DomainException("A província é obrigatória.");
        if (string.IsNullOrWhiteSpace(municipality))
            throw new DomainException("O município é obrigatório.");
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("O endereço (rua/bairro) é obrigatório.");

        CustomerId = customerId;
        Province = province.Trim();
        Municipality = municipality.Trim();
        Street = street.Trim();
        Reference = reference?.Trim();
        IsDefault = isDefault;
    }

    public void MarkAsDefault() => IsDefault = true;
    public void UnmarkAsDefault() => IsDefault = false;
}
