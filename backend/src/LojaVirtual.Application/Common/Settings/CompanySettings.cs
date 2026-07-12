namespace LojaVirtual.Application.Common.Settings;

/// <summary>Dados fiscais do vendedor, usados em toda fatura emitida.</summary>
public class CompanySettings
{
    public const string SectionName = "Company";

    public string Name { get; set; } = "Loja Virtual, Lda.";
    public string Nif { get; set; } = "5000000000";
}
