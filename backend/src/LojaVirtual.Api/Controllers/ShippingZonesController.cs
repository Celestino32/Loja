using LojaVirtual.Application.Shipping;
using LojaVirtual.Application.Shipping.Commands;
using LojaVirtual.Application.Shipping.Queries;
using LojaVirtual.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/shipping-zones")]
public class ShippingZonesController(ISender mediator) : ControllerBase
{
    /// <summary>Listagem pública das zonas ativas (usada no checkout); staff pode ver todas.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ShippingZoneDto>>> GetAll([FromQuery] bool onlyActive = true)
    {
        var isStaff = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Gerente) || User.IsInRole(Roles.Vendedor);
        return Ok(await mediator.Send(new GetShippingZonesQuery(onlyActive || !isStaff)));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<ActionResult<Guid>> Create(CreateShippingZoneCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<IActionResult> Update(Guid id, UpdateShippingZoneCommand command)
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
        await mediator.Send(new SetShippingZoneActiveCommand(id, isActive));
        return NoContent();
    }
}
