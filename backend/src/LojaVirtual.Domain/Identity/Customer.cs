using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Identity;

/// <summary>
/// Perfil de cliente da loja, vinculado ao usuário de autenticação (ApplicationUser) pelo UserId.
/// </summary>
public class Customer : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string FullName { get; private set; } = default!;
    public string? Nif { get; private set; }
    public string Phone { get; private set; } = default!;

    private readonly List<CustomerAddress> _addresses = [];
    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

    private Customer()
    {
    }

    public Customer(Guid userId, string fullName, string phone, string? nif = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("O nome do cliente é obrigatório.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("O telefone do cliente é obrigatório.");

        UserId = userId;
        FullName = fullName.Trim();
        Phone = phone.Trim();
        Nif = nif?.Trim();
    }

    public void UpdateProfile(string fullName, string phone, string? nif)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("O nome do cliente é obrigatório.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("O telefone do cliente é obrigatório.");

        FullName = fullName.Trim();
        Phone = phone.Trim();
        Nif = nif?.Trim();
        Touch();
    }

    public CustomerAddress AddAddress(string province, string municipality, string street, string? reference, bool isDefault = false)
    {
        if (isDefault)
        {
            foreach (var address in _addresses)
                address.UnmarkAsDefault();
        }

        var newAddress = new CustomerAddress(Id, province, municipality, street, reference, isDefault || _addresses.Count == 0);
        _addresses.Add(newAddress);
        Touch();
        return newAddress;
    }
}
