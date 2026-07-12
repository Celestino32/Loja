using LojaVirtual.Domain.Common;

namespace LojaVirtual.Domain.Reviews;

/// <summary>
/// Avaliação de um produto, sempre vinculada a um pedido concluído do cliente — não existe
/// avaliação sem compra confirmada. Um mesmo pedido só pode gerar uma avaliação por produto.
/// </summary>
public class Review : AuditableEntity
{
    public Guid ProductId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }

    /// <summary>Snapshot do nome do cliente no momento da avaliação, para exibição pública sem expor o cadastro.</summary>
    public string CustomerName { get; private set; } = default!;

    private Review()
    {
    }

    public Review(Guid productId, Guid customerId, Guid orderId, int rating, string? comment, string customerName)
    {
        if (rating is < 1 or > 5)
            throw new DomainException("A nota da avaliação deve ser entre 1 e 5.");
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("O nome do cliente é obrigatório na avaliação.");

        ProductId = productId;
        CustomerId = customerId;
        OrderId = orderId;
        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        CustomerName = customerName.Trim();
    }
}
