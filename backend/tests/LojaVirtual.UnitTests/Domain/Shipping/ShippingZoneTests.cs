using LojaVirtual.Domain.Common;
using LojaVirtual.Domain.Shipping;

namespace LojaVirtual.UnitTests.Domain.Shipping;

public class ShippingZoneTests
{
    [Fact]
    public void Constructor_WithNegativeCost_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new ShippingZone("Luanda", "Luanda", -100m));
    }

    [Fact]
    public void Constructor_WithEmptyProvince_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new ShippingZone("", "Luanda", 3500m));
    }

    [Fact]
    public void Deactivate_ThenActivate_TogglesState()
    {
        var zone = new ShippingZone("Luanda", "Luanda", 3500m);

        zone.Deactivate();
        Assert.False(zone.IsActive);

        zone.Activate();
        Assert.True(zone.IsActive);
    }

    [Fact]
    public void Update_ChangesCostAndLocation()
    {
        var zone = new ShippingZone("Luanda", "Luanda", 3500m);

        zone.Update("Luanda", "Talatona", 4000m);

        Assert.Equal("Talatona", zone.Municipality);
        Assert.Equal(4000m, zone.Cost);
    }
}
