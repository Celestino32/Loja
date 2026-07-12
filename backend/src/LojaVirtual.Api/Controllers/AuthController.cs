using LojaVirtual.Api.Contracts.Auth;
using LojaVirtual.Application.Common.Interfaces;
using LojaVirtual.Domain.Identity;
using LojaVirtual.Infrastructure.Auth;
using LojaVirtual.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    JwtTokenGenerator tokenGenerator,
    IApplicationDbContext context) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        var roles = await userManager.GetRolesAsync(user);
        var (token, expiresAtUtc) = tokenGenerator.GenerateToken(user, roles);

        return Ok(new AuthResponse(token, expiresAtUtc, user.FullName, user.Email!, [.. roles]));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> RegisterCustomer(RegisterCustomerRequest request, CancellationToken cancellationToken)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Conflict(new { message = "Já existe uma conta com este e-mail." });

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        var customer = new Customer(user.Id, request.FullName, request.Phone, request.Nif);
        context.Customers.Add(customer);
        await context.SaveChangesAsync(cancellationToken);

        var (token, expiresAtUtc) = tokenGenerator.GenerateToken(user, []);
        return Ok(new AuthResponse(token, expiresAtUtc, user.FullName, user.Email!, []));
    }

    /// <summary>Apenas o Admin cria contas de funcionários (Gerente/Vendedor). Sem self-signup de staff.</summary>
    [HttpPost("staff")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> CreateStaff(CreateStaffRequest request)
    {
        if (!Roles.All.Contains(request.Role))
            return BadRequest(new { message = $"Papel inválido. Valores aceites: {string.Join(", ", Roles.All)}." });

        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Conflict(new { message = "Já existe uma conta com este e-mail." });

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        await userManager.AddToRoleAsync(user, request.Role);

        return CreatedAtAction(nameof(Login), new { user.Id, user.Email, request.Role });
    }
}
