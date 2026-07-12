using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Payments;
using Microsoft.Extensions.Options;

namespace LojaVirtual.Infrastructure.Payments;

/// <summary>
/// Transferência bancária: não há confirmação automática — o financeiro confirma manualmente
/// no painel admin depois de ver o crédito na conta (endpoint de confirmação em Payments).
/// </summary>
public class BankTransferPaymentGateway(IOptions<BankTransferSettings> settings) : IPaymentGateway
{
    private readonly BankTransferSettings _settings = settings.Value;

    public PaymentMethod Method => PaymentMethod.TransferenciaBancaria;

    public Task<PaymentInitiationResult> InitiateAsync(Guid orderId, string orderNumber, decimal amount, CancellationToken cancellationToken)
    {
        var instructions =
            $"Transfira o valor total para: {_settings.BankName}, titular {_settings.AccountHolder}, " +
            $"IBAN {_settings.Iban}. Use '{orderNumber}' como referência da transferência. " +
            "O pedido é confirmado manualmente após verificação do crédito na conta.";

        return Task.FromResult(new PaymentInitiationResult(ExternalReference: null, Instructions: instructions));
    }
}
