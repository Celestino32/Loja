using LojaVirtual.Api.Common;
using LojaVirtual.Application.Invoicing;
using LojaVirtual.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize]
public class InvoicesController(ISender mediator, IInvoicePdfGenerator pdfGenerator, ISaftExporter saftExporter) : ControllerBase
{
    /// <summary>
    /// PDF da fatura de um pedido. O mesmo endpoint serve tanto a via original quanto a
    /// "segunda via" (?segundaVia=true) — os dados são sempre os mesmos, a fatura é imutável.
    /// </summary>
    [HttpGet("order/{orderId:guid}/pdf")]
    public async Task<IActionResult> GetPdfByOrder(Guid orderId, [FromQuery] bool segundaVia = false)
    {
        var isStaff = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Gerente) || User.IsInRole(Roles.Vendedor);
        var requestingUserId = isStaff ? (Guid?)null : User.GetUserId();

        var data = await mediator.Send(new GetInvoiceDataForOrderQuery(orderId, requestingUserId));
        var pdfBytes = pdfGenerator.Generate(data.Invoice, data.Items, segundaVia);

        var fileName = $"{data.Invoice.FullNumber.Replace(" ", "_").Replace("/", "-")}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

    /// <summary>
    /// Exportação SAF-T (AO) das faturas emitidas num período. Estrutura best-effort baseada
    /// no padrão OCDE, não validada contra o XSD oficial da AGT — ver <see cref="XmlSaftExporter"/>.
    /// </summary>
    [HttpGet("export/saft")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
    public async Task<IActionResult> ExportSaft([FromQuery] DateOnly from, [FromQuery] DateOnly to)
    {
        var data = await mediator.Send(new GetSaftExportDataQuery(from, to));
        var xmlBytes = saftExporter.GenerateXml(data);

        var fileName = $"SAFT_AO_{from:yyyyMMdd}_{to:yyyyMMdd}.xml";
        return File(xmlBytes, "application/xml", fileName);
    }
}
