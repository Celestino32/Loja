using LojaVirtual.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Settings.Queries;

public record GetStoreSettingsQuery : IRequest<StoreSettingsDto>;

public class GetStoreSettingsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetStoreSettingsQuery, StoreSettingsDto>
{
    public async Task<StoreSettingsDto> Handle(GetStoreSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.StoreSettings.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        return new StoreSettingsDto(settings?.StoreName ?? "Loja Virtual", settings?.LogoUrl);
    }
}
