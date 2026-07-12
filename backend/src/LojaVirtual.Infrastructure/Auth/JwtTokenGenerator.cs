using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LojaVirtual.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LojaVirtual.Infrastructure.Auth;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
{
    private readonly JwtSettings _settings = jwtOptions.Value;

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user, IList<string> roles)
    {
        // Claims curtas ("name", "role") propositalmente em vez de ClaimTypes.Name/Role:
        // o ASP.NET remapeia "role" -> ClaimTypes.Role automaticamente na validação (mantendo
        // [Authorize(Roles=...)] e User.IsInRole() a funcionar), mas mantém o payload do JWT
        // legível para o frontend, que decodifica o token manualmente sem essa camada.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("name", user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
