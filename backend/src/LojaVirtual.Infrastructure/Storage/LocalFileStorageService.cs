using LojaVirtual.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace LojaVirtual.Infrastructure.Storage;

/// <summary>
/// Grava arquivos em disco, sob um volume Docker persistente (ver docker-compose.yml).
/// Os arquivos ficam acessíveis publicamente via middleware de arquivos estáticos em
/// "/uploads" (configurado em Program.cs). Trocar para um object storage na nuvem no futuro
/// significa apenas escrever uma nova implementação de <see cref="IFileStorageService"/>.
/// </summary>
public class LocalFileStorageService(IOptions<FileStorageSettings> settings) : IFileStorageService
{
    private const string PublicPrefix = "uploads";
    private readonly string _rootPath = settings.Value.RootPath;

    public async Task<string> SaveAsync(Stream content, string fileName, string category, CancellationToken cancellationToken)
    {
        var safeCategory = SanitizeSegment(category);
        var categoryPath = Path.Combine(_rootPath, safeCategory);
        Directory.CreateDirectory(categoryPath);

        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(categoryPath, uniqueName);

        await using (var fileStream = new FileStream(fullPath, FileMode.CreateNew))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        return $"/{PublicPrefix}/{safeCategory}/{uniqueName}";
    }

    public void Delete(string url)
    {
        var prefix = $"/{PublicPrefix}/";
        if (!url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return;

        var relative = url[prefix.Length..].Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, relative));

        // Nunca apaga nada fora da pasta de uploads, mesmo que a URL tenha sido manipulada.
        if (!fullPath.StartsWith(Path.GetFullPath(_rootPath), StringComparison.OrdinalIgnoreCase))
            return;

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private static string SanitizeSegment(string segment) =>
        string.Concat(segment.Where(c => char.IsLetterOrDigit(c) || c is '-' or '_')).ToLowerInvariant();
}
