namespace LojaVirtual.Api.Contracts.Auth;

public record LoginRequest(string Email, string Password);

public record RegisterCustomerRequest(string FullName, string Email, string Password, string Phone, string? Nif);

public record CreateStaffRequest(string FullName, string Email, string Password, string Role);

public record AuthResponse(string Token, DateTime ExpiresAtUtc, string FullName, string Email, List<string> Roles);
