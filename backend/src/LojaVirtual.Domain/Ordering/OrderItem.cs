using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Ordering;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;

    private OrderItem()
    {
    }

    public OrderItem(Guid orderId, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("O nome do produto no item de pedido é obrigatório.");
        if (unitPrice < 0)
            throw new DomainException("O preço unitário não pode ser negativo.");
        if (quantity <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        OrderId = orderId;
        ProductId = productId;
        ProductName = productName.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
