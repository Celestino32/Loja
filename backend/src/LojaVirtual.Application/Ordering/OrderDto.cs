using LojaVirtual.Domain.Ordering;
using LojaVirtual.Domain.Payments;

namespace LojaVirtual.Application.Ordering;

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);

public record PaymentDto(
    PaymentMethod Method,
    PaymentStatus Status,
    decimal Amount,
    string? ExternalReference,
    string? Instructions,
    DateTime? ConfirmedAtUtc);

public record OrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    FulfillmentType FulfillmentType,
    string? ShippingProvince,
    string? ShippingMunicipality,
    string? ShippingStreet,
    string? ShippingReference,
    decimal ShippingCost,
    decimal DiscountTotal,
    decimal Total,
    DateTime CreatedAtUtc,
    List<OrderItemDto> Items,
    PaymentDto? Payment,
    bool HasInvoice,
    string CustomerName,
    string CustomerPhone);

public record OrderSummaryDto(Guid Id, string OrderNumber, OrderStatus Status, decimal Total, DateTime CreatedAtUtc);
