namespace LojaVirtual.Application.Common;

public static class ImageUploadRules
{
    public static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
    public const long MaxSizeBytes = 5 * 1024 * 1024;
}
