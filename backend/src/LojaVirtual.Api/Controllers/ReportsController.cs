using LojaVirtual.Application.Reports;
using LojaVirtual.Application.Reports.Queries;
using LojaVirtual.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Gerente}")]
public class ReportsController(ISender mediator) : ControllerBase
{
    [HttpGet("sales")]
    public async Task<ActionResult<SalesReportDto>> GetSales([FromQuery] DateOnly from, [FromQuery] DateOnly to)
    {
        return Ok(await mediator.Send(new GetSalesReportQuery(from, to)));
    }

    [HttpGet("top-products")]
    public async Task<ActionResult<List<TopProductDto>>> GetTopProducts(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] int limit = 10)
    {
        return Ok(await mediator.Send(new GetTopProductsQuery(from, to, limit)));
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<List<LowStockProductDto>>> GetLowStock([FromQuery] int threshold = 5)
    {
        return Ok(await mediator.Send(new GetLowStockProductsQuery(threshold)));
    }
}
