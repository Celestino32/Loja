using LojaVirtual.Api.Common;
using LojaVirtual.Application.Catalog.Products;
using LojaVirtual.Application.Ordering;
using LojaVirtual.Application.Ordering.Commands;
using LojaVirtual.Application.Ordering.Queries;
using LojaVirtual.Domain.Ordering;
using LojaVirtual.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(ISender mediator) : ControllerBase
{
    /// <summary>Finaliza a compra: valida stock/preços no servidor, cria o pedido e inicia o pagamento.</summary>
    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutResultDto>> Checkout(CheckoutRequest request)
    {
        var command = new CheckoutCommand(
            User.GetUserId(),
            [.. request.Items.Select(i => new CheckoutItem(i.ProductId, i.Quantity))],
            request.FulfillmentType,
            request.ShippingZoneId,
            request.ShippingStreet,
            request.ShippingReference,
            request.PaymentMethod,
            request.CouponCode);

        return Ok(await mediator.Send(command));
    }

    [HttpGet("mine")]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetMyOrders()
    {
        return Ok(await mediator.Send(new GetMyOrdersQuery(User.GetUserId())));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var isStaff = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Gerente) || User.IsInRole(Roles.Vendedor);
        var requestingUserId = isStaff ? (Guid?)null : User.GetUserId();
        return Ok(await mediator.Send(new GetOrderByIdQuery(id, requestingUserId)));
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<ActionResult<PagedResult<OrderSummaryDto>>> GetAll(
        [FromQuery] OrderStatus? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return Ok(await mediator.Send(new GetAllOrdersQuery(status, search, page, pageSize)));
    }

    /// <summary>Confirmação manual de pagamento (Referência/Transferência) pelo financeiro.</summary>
    [HttpPatch("{id:guid}/confirm-payment")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<IActionResult> ConfirmPayment(Guid id)
    {
        await mediator.Send(new ConfirmPaymentCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:guid}/start-preparing")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<IActionResult> StartPreparing(Guid id)
    {
        await mediator.Send(new StartPreparingOrderCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:guid}/mark-ready")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<IActionResult> MarkReady(Guid id)
    {
        await mediator.Send(new MarkOrderReadyCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:guid}/complete")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await mediator.Send(new CompleteOrderCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await mediator.Send(new CancelOrderCommand(id));
        return NoContent();
    }
}

public record CheckoutItemRequest(Guid ProductId, int Quantity);

public record CheckoutRequest(
    List<CheckoutItemRequest> Items,
    FulfillmentType FulfillmentType,
    Guid? ShippingZoneId,
    string? ShippingStreet,
    string? ShippingReference,
    Domain.Payments.PaymentMethod PaymentMethod,
    string? CouponCode = null);
