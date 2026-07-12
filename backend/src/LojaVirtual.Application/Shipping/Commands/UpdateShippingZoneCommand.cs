using FluentValidation;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Shipping.Commands;

public record UpdateShippingZoneCommand(Guid Id, string Province, string Municipality, decimal Cost) : IRequest;

public class UpdateShippingZoneCommandValidator : AbstractValidator<UpdateShippingZoneCommand>
{
    public UpdateShippingZoneCommandValidator()
    {
        RuleFor(x => x.Province).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Municipality).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}

public class UpdateShippingZoneCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateShippingZoneCommand>
{
    public async Task Handle(UpdateShippingZoneCommand request, CancellationToken cancellationToken)
    {
        var zone = await context.ShippingZones.FirstOrDefaultAsync(z => z.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Shipping.ShippingZone), request.Id);

        zone.Update(request.Province, request.Municipality, request.Cost);
        await context.SaveChangesAsync(cancellationToken);
    }
}
