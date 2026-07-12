using FluentValidation;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Shipping;
using MediatR;

namespace LojaVirtual.Application.Shipping.Commands;

public record CreateShippingZoneCommand(string Province, string Municipality, decimal Cost) : IRequest<Guid>;

public class CreateShippingZoneCommandValidator : AbstractValidator<CreateShippingZoneCommand>
{
    public CreateShippingZoneCommandValidator()
    {
        RuleFor(x => x.Province).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Municipality).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}

public class CreateShippingZoneCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateShippingZoneCommand, Guid>
{
    public async Task<Guid> Handle(CreateShippingZoneCommand request, CancellationToken cancellationToken)
    {
        var zone = new ShippingZone(request.Province, request.Municipality, request.Cost);
        context.ShippingZones.Add(zone);
        await context.SaveChangesAsync(cancellationToken);
        return zone.Id;
    }
}
