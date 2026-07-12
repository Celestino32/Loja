namespace LojaVirtual.Application.Customers;

public record CustomerAddressDto(Guid Id, string Province, string Municipality, string Street, string? Reference, bool IsDefault);

public record CustomerDto(Guid Id, string FullName, string Phone, string? Nif, List<CustomerAddressDto> Addresses);
