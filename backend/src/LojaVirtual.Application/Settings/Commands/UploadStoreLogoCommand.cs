using FluentValidation;
using LojaVirtual.Application.Common;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Application.Settings.Commands;

public record UploadStoreLogoCommand(Stream Content, string FileName, string ContentType, long Length) : IRequest<string>;

public class UploadStoreLogoCommandValidator : AbstractValidator<UploadStoreLogoCommand>
{
    public UploadStoreLogoCommandValidator()
    {
        RuleFor(x => x.ContentType)
            .Must(ct => ImageUploadRules.AllowedContentTypes.Contains(ct))
            .WithMessage($"Formato inválido. Aceites: {string.Join(", ", ImageUploadRules.AllowedContentTypes)}.");

        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("Arquivo vazio.")
            .LessThanOrEqualTo(ImageUploadRules.MaxSizeBytes)
            .WithMessage($"Arquivo maior que o limite de {ImageUploadRules.MaxSizeBytes / 1024 / 1024}MB.");
    }
}

public class UploadStoreLogoCommandHandler(IApplicationDbContext context, IFileStorageService storage)
    : IRequestHandler<UploadStoreLogoCommand, string>
{
    public async Task<string> Handle(UploadStoreLogoCommand request, CancellationToken cancellationToken)
    {
        var settings = await context.StoreSettings.FirstOrDefaultAsync(cancellationToken);
        var previousLogoUrl = settings?.LogoUrl;

        var url = await storage.SaveAsync(request.Content, request.FileName, "branding", cancellationToken);

        if (settings is null)
        {
            settings = new StoreSettings("Loja Virtual");
            context.StoreSettings.Add(settings);
        }

        settings.UpdateLogo(url);
        await context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(previousLogoUrl))
            storage.Delete(previousLogoUrl);

        return url;
    }
}
