namespace LojaVirtual.Infrastructure.Payments;

public class BankTransferSettings
{
    public const string SectionName = "Payments:BankTransfer";

    public string BankName { get; set; } = "Banco a definir";
    public string AccountHolder { get; set; } = "Loja Virtual, Lda.";
    public string Iban { get; set; } = "AO06 0000 0000 0000 0000 0000 0";
}

/// <summary>
/// Credenciais do facilitador (AppyPay/IZI Pay) para Multicaixa Express via GPO/EMIS.
/// Vazias por padrão — sem elas, <see cref="AppyPayGateway"/> falha alto e claro em vez de
/// simular sucesso. Preencha após concluir o cadastro de comerciante junto ao facilitador.
/// </summary>
public class AppyPaySettings
{
    public const string SectionName = "Payments:AppyPay";

    public string BaseUrl { get; set; } = "https://api.appypay.co.ao";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string PosId { get; set; } = string.Empty;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);
}
