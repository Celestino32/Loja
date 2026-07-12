using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Customers.Queries;

public record GetMyProfileQuery(Guid UserId) : IRequest<CustomerDto>;

public class GetMyProfileQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyProfileQuery, CustomerDto>
{
    public async Task<CustomerDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.AsNoTracking()
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), request.UserId);

        return new CustomerDto(
            customer.Id,
            customer.FullName,
            customer.Phone,
            customer.Nif,
            customer.Addresses
                .Select(a => new CustomerAddressDto(a.Id, a.Province, a.Municipality, a.Street, a.Reference, a.IsDefault))
                .ToList());
    }
}
