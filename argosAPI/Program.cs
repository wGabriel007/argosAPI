using argos.Infra.Data;
using argos.Infra.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

// Carrega o .env antes de criar o builder para que ASPNETCORE_ENVIRONMENT
// também possa ser definido pelo arquivo.
var contentRootPath = AppContext.BaseDirectory;
var localEnvPath = Path.Combine(contentRootPath, ".env");
var solutionEnvPath = Path.GetFullPath(
    Path.Combine(contentRootPath, "..", ".env"));
var envPath = File.Exists(localEnvPath) ? localEnvPath : solutionEnvPath;

if (File.Exists(envPath))
{
    DotNetEnv.Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);
var isHomologacao = builder.Environment.IsEnvironment("Homologacao");
var corsEnabled = bool.TryParse(
    Environment.GetEnvironmentVariable("CORS_ENABLED"),
    out var configuredCorsEnabled)
    && configuredCorsEnabled;

var frontendOrigins = (
    Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")
    ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var cookieSameSite = Enum.TryParse<SameSiteMode>(
    Environment.GetEnvironmentVariable("AUTH_COOKIE_SAMESITE"),
    true,
    out var configuredSameSite)
    ? configuredSameSite
    : isHomologacao ? SameSiteMode.None : SameSiteMode.Lax;

var cookieSecure = bool.TryParse(
    Environment.GetEnvironmentVariable("AUTH_COOKIE_SECURE"),
    out var configuredSecure)
    ? configuredSecure
    : isHomologacao;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Registra o contexto para que os controllers possam consultar o PostgreSQL.
builder.Services.AddDbContext<ArgosDbContext>(options =>
    options.UseNpgsql(Conexao.StringConexao));

// A aplicação usa cookie HttpOnly, conforme o contrato do frontend.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "argos.auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.Path = "/";
        options.Cookie.SameSite = cookieSameSite;
        options.Cookie.SecurePolicy = cookieSecure
            ? CookieSecurePolicy.Always
            : CookieSecurePolicy.SameAsRequest;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()
    || app.Environment.IsEnvironment("Homologacao"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Deve ser executado antes da autenticação para permitir preflight e cookies
// nas requisições feitas pelo frontend.
if (corsEnabled)
{
    app.UseCors("Frontend");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
