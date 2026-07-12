namespace LojaVirtual.Application.Common.Interfaces;

/// <summary>
/// Abstração de armazenamento de arquivos enviados (imagens de produto, logotipo). A
/// implementação padrão grava em disco local (volume Docker); trocar para um object storage
/// na nuvem (S3, Azure Blob) mais tarde não deve exigir mudanças fora da Infrastructure.
/// </summary>
public interface IFileStorageService
{
    /// <summary>Grava o conteúdo sob a categoria informada e devolve a URL pública relativa (ex: "/uploads/products/xxx.jpg").</summary>
    Task<string> SaveAsync(Stream content, string fileName, string category, CancellationToken cancellationToken);

    /// <summary>Remove o arquivo correspondente à URL pública; não falha se o arquivo já não existir.</summary>
    void Delete(string url);
}
