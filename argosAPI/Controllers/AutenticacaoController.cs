using System.Security.Claims;
using argosAPI.Helpers;
using argosAPI.Models;
using argos.Infra.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace argosAPI.Controllers;

/// <summary>Login e encerramento da sessão por cookie HttpOnly.</summary>
[ApiController]
[Route("autenticacao")]
public sealed class AutenticacaoController(ArgosDbContext db) : ControllerBase
{
    [HttpPost("entrar")]
    public async Task<IActionResult> Enter(LoginRequest request, CancellationToken ct)
    {
        var user = await db.Usuarios.SingleOrDefaultAsync(x => x.Email == request.Email, ct);
        if (user is null || user.Status == 0 || !string.Equals(user.SenhaHash, ApiRules.Sha256(request.Password), StringComparison.OrdinalIgnoreCase))
            return Unauthorized(new ErrorResponse("E-mail ou senha inválidos."));

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.Nome), new Claim(ClaimTypes.Email, user.Email) };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        return Ok(new LoginResponse(user.Nome, user.Email));
    }

    [HttpPost("sair")]
    public async Task<IActionResult> Exit()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}
