namespace LojaVirtual.Infrastructure.Storage;

public class FileStorageSettings
{
    public const string SectionName = "FileStorage";

    /// <summary>Pasta raiz onde os arquivos enviados são gravados. Em Docker aponta para o
    /// volume persistente montado em /app/uploads (ver docker-compose.yml).</summary>
    public string RootPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "uploads");
}
