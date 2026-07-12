using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LojaVirtual.Api.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (value is null || !Guid.TryParse(value, out var userId))
            throw new InvalidOperationException("Não foi possível identificar o usuário autenticado.");
        return userId;
    }
}
