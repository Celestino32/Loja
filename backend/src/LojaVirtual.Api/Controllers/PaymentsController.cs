using LojaVirtual.Application.Ordering.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Webhook de confirmação do facilitador Multicaixa Express (AppyPay/IZI Pay).
    /// ATENÇÃO: antes de usar em produção, é obrigatório validar a assinatura/segredo do
    /// webhook conforme a documentação oficial do facilitador — sem isso, qualquer requisição
    /// externa poderia forjar uma confirmação de pagamento. Esta implementação ainda não faz
    /// essa validação porque o formato exato (header, algoritmo) depende da conta real.
    /// </summary>
    [HttpPost("appypay/webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> AppyPayWebhook(AppyPayWebhookPayload payload)
    {
        await mediator.Send(new ConfirmPaymentCommand(payload.OrderId, payload.ChargeId));
        return Ok();
    }
}

public record AppyPayWebhookPayload(Guid OrderId, string ChargeId);
