using LojaVirtual.Application.Coupons;
using LojaVirtual.Application.Coupons.Commands;
using LojaVirtual.Application.Coupons.Queries;
using LojaVirtual.Domain.Coupons;
using LojaVirtual.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/coupons")]
public class CouponsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<ActionResult<List<CouponDto>>> GetAll()
    {
        return Ok(await mediator.Send(new GetCouponsQuery()));
    }

    /// <summary>Pré-visualização do desconto no carrinho, antes do checkout definitivo.</summary>
    [HttpGet("validate")]
    [AllowAnonymous]
    public async Task<ActionResult<CouponValidationResultDto>> Validate([FromQuery] string code, [FromQuery] decimal subtotal)
    {
        return Ok(await mediator.Send(new ValidateCouponQuery(code, subtotal)));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<ActionResult<Guid>> Create(CreateCouponCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<IActionResult> Update(Guid id, UpdateCouponCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { message = "O id da rota não corresponde ao id do corpo da requisição." });

        await mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] bool isActive)
    {
        await mediator.Send(new SetCouponActiveCommand(id, isActive));
        return NoContent();
    }
}
