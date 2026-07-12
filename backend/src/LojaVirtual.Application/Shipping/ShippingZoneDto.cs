namespace LojaVirtual.Application.Shipping;

public record ShippingZoneDto(Guid Id, string Province, string Municipality, decimal Cost, bool IsActive);
