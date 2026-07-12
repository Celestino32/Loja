using FluentValidation;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Customers.Commands;

public record AddCustomerAddressCommand(
    Guid UserId,
    string Province,
    string Municipality,
    string Street,
    string? Reference,
    bool IsDefault) : IRequest<Guid>;

public class AddCustomerAddressCommandValidator : AbstractValidator<AddCustomerAddressCommand>
{
    public AddCustomerAddressCommandValidator()
    {
        RuleFor(x => x.Province).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Municipality).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(300);
    }
}

public class AddCustomerAddressCommandHandler(IApplicationDbContext context) : IRequestHandler<AddCustomerAddressCommand, Guid>
{
    public async Task<Guid> Handle(AddCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), request.UserId);

        var address = customer.AddAddress(request.Province, request.Municipality, request.Street, request.Reference, request.IsDefault);

        // Mesmo motivo do upload de imagem de produto: Customer já está rastreado, então o novo
        // endereço (Guid gerado no cliente) precisa ser marcado como Added explicitamente.
        context.CustomerAddresses.Add(address);

        await context.SaveChangesAsync(cancellationToken);
        return address.Id;
    }
}
