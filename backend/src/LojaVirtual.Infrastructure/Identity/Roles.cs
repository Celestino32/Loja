namespace LojaVirtual.Infrastructure.Identity;

/// <summary>Papéis de staff interno. Clientes finais não recebem nenhum destes papéis.</summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Gerente = "Gerente";
    public const string Vendedor = "Vendedor";

    public static readonly string[] All = [Admin, Gerente, Vendedor];

    /// <summary>Papéis com permissão para gerenciar catálogo (produtos/categorias/estoque).</summary>
    public static readonly string[] CatalogManagers = [Admin, Gerente, Vendedor];
}
