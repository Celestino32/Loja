using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Ordering;

namespace LojaVirtual.UnitTests.Domain.Ordering;

public class OrderTests
{
    private static Order CreateOrderWithItem()
    {
        var order = new Order("PED-0001", Guid.NewGuid(), FulfillmentType.RetiradaNaLoja);
        order.AddItem(Guid.NewGuid(), "Produto Teste", 100m, 2);
        return order;
    }

    [Fact]
    public void AddItem_RecalculatesTotal()
    {
        var order = CreateOrderWithItem();

        Assert.Equal(200m, order.Total);
    }

    [Fact]
    public void RetiradaNaLoja_HasNoShippingCost()
    {
        var order = new Order("PED-0002", Guid.NewGuid(), FulfillmentType.RetiradaNaLoja, shippingCost: 5000m);

        Assert.Equal(0m, order.ShippingCost);
    }

    [Fact]
    public void MarkAsPaid_WithoutItems_ThrowsDomainException()
    {
        var order = new Order("PED-0003", Guid.NewGuid(), FulfillmentType.RetiradaNaLoja);

        Assert.Throws<DomainException>(order.MarkAsPaid);
    }

    [Fact]
    public void MarkAsPaid_TwiceInARow_ThrowsDomainException()
    {
        var order = CreateOrderWithItem();
        order.MarkAsPaid();

        Assert.Throws<DomainException>(order.MarkAsPaid);
    }

    [Fact]
    public void FullHappyPath_TransitionsThroughAllStates()
    {
        var order = CreateOrderWithItem();

        order.MarkAsPaid();
        order.StartPreparing();
        order.MarkReadyForDeliveryOrPickup();
        order.Complete();

        Assert.Equal(OrderStatus.Concluido, order.Status);
    }

    [Fact]
    public void Cancel_AfterCompleted_ThrowsDomainException()
    {
        var order = CreateOrderWithItem();
        order.MarkAsPaid();
        order.StartPreparing();
        order.MarkReadyForDeliveryOrPickup();
        order.Complete();

        Assert.Throws<DomainException>(order.Cancel);
    }

    [Fact]
    public void AddItem_AfterPaid_ThrowsDomainException()
    {
        var order = CreateOrderWithItem();
        order.MarkAsPaid();

        Assert.Throws<DomainException>(() => order.AddItem(Guid.NewGuid(), "Outro Produto", 50m, 1));
    }

    [Fact]
    public void Entrega_WithoutShippingAddress_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Order("PED-0004", Guid.NewGuid(), FulfillmentType.Entrega, shippingCost: 3500m));
    }

    [Fact]
    public void Entrega_WithShippingAddress_SetsFieldsAndCost()
    {
        var order = new Order(
            "PED-0005", Guid.NewGuid(), FulfillmentType.Entrega, shippingCost: 3500m,
            shippingProvince: "Luanda", shippingMunicipality: "Talatona", shippingStreet: "Rua Teste, 123");

        Assert.Equal(3500m, order.ShippingCost);
        Assert.Equal("Luanda", order.ShippingProvince);
        Assert.Equal("Talatona", order.ShippingMunicipality);
    }
}
