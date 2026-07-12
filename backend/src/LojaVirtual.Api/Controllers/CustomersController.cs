using LojaVirtual.Api.Common;
using LojaVirtual.Application.Customers;
using LojaVirtual.Application.Customers.Commands;
using LojaVirtual.Application.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/customers/me")]
[Authorize]
public class CustomersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CustomerDto>> GetMyProfile()
    {
        return Ok(await mediator.Send(new GetMyProfileQuery(User.GetUserId())));
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<Guid>> AddAddress(AddAddressRequest request)
    {
        var id = await mediator.Send(new AddCustomerAddressCommand(
            User.GetUserId(), request.Province, request.Municipality, request.Street, request.Reference, request.IsDefault));

        return CreatedAtAction(nameof(GetMyProfile), new { id }, id);
    }
}

public record AddAddressRequest(string Province, string Municipality, string Street, string? Reference, bool IsDefault);
