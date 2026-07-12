using System.Security.Cryptography;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Payments;
using Microsoft.Extensions.Configuration;

namespace LojaVirtual.Infrastructure.Payments;

/// <summary>
/// Referência de pagamento (Multicaixa ATM/Internet Banking). A "Entidade" real só é obtida
/// registando a loja como aceitador junto à EMIS — o valor de configuração é um placeholder
/// até essa adesão ser concluída. A referência gerada aqui não é validada por nenhum banco;
/// a confirmação do pagamento continua manual (reconciliação pelo financeiro).
/// </summary>
public class ReferencePaymentGateway(IConfiguration configuration) : IPaymentGateway
{
    public PaymentMethod Method => PaymentMethod.ReferenciaDePagamento;

    public Task<PaymentInitiationResult> InitiateAsync(Guid orderId, string orderNumber, decimal amount, CancellationToken cancellationToken)
    {
        var entity = configuration["Payments:Reference:Entity"] ?? "00000";
        var reference = GenerateReference();

        var instructions =
            $"Pague em qualquer ATM Multicaixa ou Internet Banking. Entidade: {entity} · Referência: {reference} · " +
            $"Valor: {amount:N2} Kz. O pedido é confirmado manualmente após verificação do pagamento.";

        return Task.FromResult(new PaymentInitiationResult(ExternalReference: reference, Instructions: instructions));
    }

    private static string GenerateReference()
    {
        Span<byte> buffer = stackalloc byte[4];
        RandomNumberGenerator.Fill(buffer);
        var value = BitConverter.ToUInt32(buffer) % 1_000_000_000u;
        return value.ToString("D9");
    }
}
