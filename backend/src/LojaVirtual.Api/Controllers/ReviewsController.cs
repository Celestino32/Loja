using LojaVirtual.Api.Common;
using LojaVirtual.Application.Reviews;
using LojaVirtual.Application.Reviews.Commands;
using LojaVirtual.Application.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(ISender mediator) : ControllerBase
{
    [HttpGet("product/{productId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductReviewsDto>> GetByProduct(Guid productId)
    {
        return Ok(await mediator.Send(new GetProductReviewsQuery(productId)));
    }

    [HttpGet("mine")]
    [Authorize]
    public async Task<ActionResult<List<Guid>>> GetMineForOrder([FromQuery] Guid orderId)
    {
        return Ok(await mediator.Send(new GetMyReviewedProductsQuery(User.GetUserId(), orderId)));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> Create(CreateReviewRequest request)
    {
        var id = await mediator.Send(new CreateReviewCommand(User.GetUserId(), request.ProductId, request.OrderId, request.Rating, request.Comment));
        return CreatedAtAction(nameof(GetByProduct), new { productId = request.ProductId }, id);
    }
}

public record CreateReviewRequest(Guid ProductId, Guid OrderId, int Rating, string? Comment);
