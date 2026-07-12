using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Settings;

/// <summary>
/// Configurações gerais da loja (linha única). Hoje só guarda o logotipo, mas é o lugar
/// natural para crescer (nome de exibição, cores de marca, etc.) sem precisar de migrations
/// separadas por campo.
/// </summary>
public class StoreSettings : AuditableEntity
{
    public string StoreName { get; private set; } = default!;
    public string? LogoUrl { get; private set; }

    private StoreSettings()
    {
    }

    public StoreSettings(string storeName)
    {
        SetStoreName(storeName);
    }

    public void UpdateStoreName(string storeName)
    {
        SetStoreName(storeName);
        Touch();
    }

    public void UpdateLogo(string logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
            throw new DomainException("A URL do logotipo é obrigatória.");
        LogoUrl = logoUrl.Trim();
        Touch();
    }

    private void SetStoreName(string storeName)
    {
        if (string.IsNullOrWhiteSpace(storeName))
            throw new DomainException("O nome da loja é obrigatório.");
        StoreName = storeName.Trim();
    }
}
