using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Payments;
using Microsoft.Extensions.Options;

namespace LojaVirtual.Infrastructure.Payments;

/// <summary>
/// Multicaixa Express via facilitador certificado pela EMIS (ex: AppyPay). A autenticação
/// OAuth2 client-credentials segue o padrão documentado publicamente pelo facilitador, mas o
/// endpoint/payload exatos de criação de cobrança (rota, nomes de campos) DEVEM ser conferidos
/// na documentação oficial obtida após o cadastro como comerciante — não foram verificados
/// contra uma conta real e não devem ser considerados definitivos antes disso.
/// Sem credenciais configuradas, falha alto e claro em vez de simular sucesso.
/// </summary>
public class AppyPayGateway(HttpClient httpClient, IOptions<AppyPaySettings> settings) : IPaymentGateway
{
    private readonly AppyPaySettings _settings = settings.Value;

    public PaymentMethod Method => PaymentMethod.MulticaixaExpress;

    public async Task<PaymentInitiationResult> InitiateAsync(Guid orderId, string orderNumber, decimal amount, CancellationToken cancellationToken)
    {
        if (!_settings.IsConfigured)
        {
            throw new PaymentGatewayException(
                "Multicaixa Express ainda não está configurado (faltam credenciais do facilitador AppyPay/IZI Pay). " +
                "Escolha Referência de Pagamento ou Transferência Bancária, ou configure Payments:AppyPay no backend.");
        }

        var accessToken = await GetAccessTokenAsync(cancellationToken);

        // TODO: confirmar o endpoint e o formato exato do payload na documentação oficial do
        // facilitador antes de usar em produção — esta é a melhor estimativa a partir do padrão
        // OAuth2 + REST documentado publicamente (POST autenticado, valor em Kwanza, referência
        // própria do comerciante para reconciliação).
        var request = new HttpRequestMessage(HttpMethod.Post, "/charges")
        {
            Headers = { { "Authorization", $"Bearer {accessToken}" } },
            Content = JsonContent.Create(new AppyPayChargeRequest(
                PosId: _settings.PosId,
                Amount: amount,
                Currency: "AOA",
                MerchantReference: orderNumber))
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new PaymentGatewayException($"O facilitador de pagamento recusou a cobrança ({(int)response.StatusCode}): {body}");
        }

        var charge = await response.Content.ReadFromJsonAsync<AppyPayChargeResponse>(cancellationToken)
            ?? throw new PaymentGatewayException("Resposta inesperada do facilitador de pagamento.");

        return new PaymentInitiationResult(
            ExternalReference: charge.Id,
            Instructions: "Confirme o pagamento na app Multicaixa Express. Você tem 90 segundos para validar com PIN ou biometria.");
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/oauth2/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret
            })
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new PaymentGatewayException("Não foi possível autenticar com o facilitador de pagamento (Multicaixa Express).");

        var token = await response.Content.ReadFromJsonAsync<AppyPayTokenResponse>(cancellationToken)
            ?? throw new PaymentGatewayException("Resposta de autenticação inesperada do facilitador de pagamento.");

        return token.AccessToken;
    }

    private record AppyPayChargeRequest(
        [property: JsonPropertyName("posId")] string PosId,
        [property: JsonPropertyName("amount")] decimal Amount,
        [property: JsonPropertyName("currency")] string Currency,
        [property: JsonPropertyName("merchantReference")] string MerchantReference);

    private record AppyPayChargeResponse([property: JsonPropertyName("id")] string Id);

    private record AppyPayTokenResponse([property: JsonPropertyName("access_token")] string AccessToken);
}
