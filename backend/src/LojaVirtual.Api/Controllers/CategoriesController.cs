using LojaVirtual.Application.Catalog.Categories;
using LojaVirtual.Application.Catalog.Categories.Commands;
using LojaVirtual.Application.Catalog.Categories.Queries;
using LojaVirtual.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ISender mediator) : ControllerBase
{
    /// <summary>Listagem pública de categorias ativas (usada pela loja).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryDto>>> GetAll([FromQuery] bool onlyActive = true)
    {
        return Ok(await mediator.Send(new GetCategoriesQuery(onlyActive)));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        return Ok(await mediator.Send(new GetCategoryByIdQuery(id)));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<ActionResult<Guid>> Create(CreateCategoryCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command)
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
        await mediator.Send(new SetCategoryActiveCommand(id, isActive));
        return NoContent();
    }
}
