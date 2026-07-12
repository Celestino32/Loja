using LojaVirtual.Application.Catalog.Products;
using LojaVirtual.Application.Catalog.Products.Commands;
using LojaVirtual.Application.Catalog.Products.Queries;
using LojaVirtual.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(ISender mediator) : ControllerBase
{
    /// <summary>Listagem pública paginada de produtos, com busca e filtro por categoria (usada pela loja).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] bool onlyActive = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Apenas staff autenticado pode pedir produtos inativos (gestão de catálogo).
        var isStaff = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Gerente) || User.IsInRole(Roles.Vendedor);
        var query = new GetProductsQuery(search, categoryId, OnlyActive: onlyActive || !isStaff, page, pageSize);
        return Ok(await mediator.Send(query));
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetBySlug(string slug)
    {
        return Ok(await mediator.Send(new GetProductBySlugQuery(slug)));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        return Ok(await mediator.Send(new GetProductByIdQuery(id)));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { message = "O id da rota não corresponde ao id do corpo da requisição." });

        await mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id:guid}/stock")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<IActionResult> AdjustStock(Guid id, [FromBody] int quantity)
    {
        await mediator.Send(new AdjustStockCommand(id, quantity));
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] bool isActive)
    {
        await mediator.Send(new SetProductActiveCommand(id, isActive));
        return NoContent();
    }

    /// <summary>Envia uma nova foto para o produto (multipart/form-data, campo "file").</summary>
    [HttpPost("{id:guid}/images")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<ProductImageDto>> UploadImage(Guid id, IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var image = await mediator.Send(new UploadProductImageCommand(id, stream, file.FileName, file.ContentType, file.Length));
        return Ok(image);
    }

    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente},{Roles.Vendedor}")]
    public async Task<IActionResult> RemoveImage(Guid id, Guid imageId)
    {
        await mediator.Send(new RemoveProductImageCommand(id, imageId));
        return NoContent();
    }
}
