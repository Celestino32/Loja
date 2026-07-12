using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Payments;

namespace LojaVirtual.Domain.Ordering;

/// <summary>
/// Pedido do cliente: máquina de estados, itens, cálculo de total e snapshot do endereço de
/// entrega (capturado no momento do checkout — mudanças posteriores no cadastro do cliente
/// não devem alterar pedidos já feitos).
/// </summary>
public class Order : AuditableEntity
{
    public string OrderNumber { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pendente;
    public FulfillmentType FulfillmentType { get; private set; }
    public decimal ShippingCost { get; private set; }
    public decimal DiscountTotal { get; private set; }
    public string? CouponCode { get; private set; }
    public decimal Total { get; private set; }

    public string? ShippingProvince { get; private set; }
    public string? ShippingMunicipality { get; private set; }
    public string? ShippingStreet { get; private set; }
    public string? ShippingReference { get; private set; }

    public Payment? Payment { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order()
    {
    }

    public Order(
        string orderNumber,
        Guid customerId,
        FulfillmentType fulfillmentType,
        decimal shippingCost = 0,
        string? shippingProvince = null,
        string? shippingMunicipality = null,
        string? shippingStreet = null,
        string? shippingReference = null)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainException("O número do pedido é obrigatório.");
        if (shippingCost < 0)
            throw new DomainException("O custo de entrega não pode ser negativo.");

        if (fulfillmentType == FulfillmentType.Entrega)
        {
            if (string.IsNullOrWhiteSpace(shippingProvince) || string.IsNullOrWhiteSpace(shippingMunicipality) || string.IsNullOrWhiteSpace(shippingStreet))
                throw new DomainException("Província, município e endereço são obrigatórios para entrega.");
        }

        OrderNumber = orderNumber.Trim();
        CustomerId = customerId;
        FulfillmentType = fulfillmentType;
        ShippingCost = fulfillmentType == FulfillmentType.RetiradaNaLoja ? 0 : shippingCost;

        if (fulfillmentType == FulfillmentType.Entrega)
        {
            ShippingProvince = shippingProvince!.Trim();
            ShippingMunicipality = shippingMunicipality!.Trim();
            ShippingStreet = shippingStreet!.Trim();
            ShippingReference = shippingReference?.Trim();
        }
    }

    public OrderItem AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        EnsureEditable();
        var item = new OrderItem(Id, productId, productName, unitPrice, quantity);
        _items.Add(item);
        RecalculateTotal();
        return item;
    }

    public void ApplyDiscount(decimal amount, string? couponCode = null)
    {
        EnsureEditable();
        if (amount < 0)
            throw new DomainException("O desconto não pode ser negativo.");
        DiscountTotal = amount;
        CouponCode = couponCode?.Trim().ToUpperInvariant();
        RecalculateTotal();
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pendente)
            throw new DomainException($"Não é possível marcar como pago um pedido no estado '{Status}'.");
        if (_items.Count == 0)
            throw new DomainException("Não é possível pagar um pedido sem itens.");
        Status = OrderStatus.Pago;
        Touch();
    }

    public void StartPreparing()
    {
        if (Status != OrderStatus.Pago)
            throw new DomainException($"Não é possível preparar um pedido no estado '{Status}'.");
        Status = OrderStatus.Preparando;
        Touch();
    }

    public void MarkReadyForDeliveryOrPickup()
    {
        if (Status != OrderStatus.Preparando)
            throw new DomainException($"Não é possível avançar um pedido no estado '{Status}'.");
        Status = OrderStatus.ProntoParaEntregaOuRetirada;
        Touch();
    }

    public void Complete()
    {
        if (Status != OrderStatus.ProntoParaEntregaOuRetirada)
            throw new DomainException($"Não é possível concluir um pedido no estado '{Status}'.");
        Status = OrderStatus.Concluido;
        Touch();
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Concluido or OrderStatus.Cancelado)
            throw new DomainException($"Não é possível cancelar um pedido no estado '{Status}'.");
        Status = OrderStatus.Cancelado;
        Touch();
    }

    private void EnsureEditable()
    {
        if (Status != OrderStatus.Pendente)
            throw new DomainException("O pedido não pode mais ser alterado neste estado.");
    }

    private void RecalculateTotal()
    {
        var itemsTotal = _items.Sum(i => i.LineTotal);
        Total = itemsTotal + ShippingCost - DiscountTotal;
        Touch();
    }
}
