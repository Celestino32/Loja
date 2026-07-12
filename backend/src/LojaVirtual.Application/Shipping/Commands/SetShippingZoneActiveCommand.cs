using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Shipping.Commands;

public record SetShippingZoneActiveCommand(Guid Id, bool IsActive) : IRequest;

public class SetShippingZoneActiveCommandHandler(IApplicationDbContext context) : IRequestHandler<SetShippingZoneActiveCommand>
{
    public async Task Handle(SetShippingZoneActiveCommand request, CancellationToken cancellationToken)
    {
        var zone = await context.ShippingZones.FirstOrDefaultAsync(z => z.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Shipping.ShippingZone), request.Id);

        if (request.IsActive)
            zone.Activate();
        else
            zone.Deactivate();

        await context.SaveChangesAsync(cancellationToken);
    }
}
