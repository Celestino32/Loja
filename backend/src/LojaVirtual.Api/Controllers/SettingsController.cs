using LojaVirtual.Application.Settings;
using LojaVirtual.Application.Settings.Commands;
using LojaVirtual.Application.Settings.Queries;
using LojaVirtual.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController(ISender mediator) : ControllerBase
{
    /// <summary>Nome e logotipo atuais da loja (usado pelo storefront e pelo admin).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<StoreSettingsDto>> Get()
    {
        return Ok(await mediator.Send(new GetStoreSettingsQuery()));
    }

    /// <summary>Envia um novo logotipo (multipart/form-data, campo "file"). Substitui o anterior.</summary>
    [HttpPost("logo")]
    [Authorize(Roles = Roles.Admin)]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<string>> UploadLogo(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var url = await mediator.Send(new UploadStoreLogoCommand(stream, file.FileName, file.ContentType, file.Length));
        return Ok(new { url });
    }
}
