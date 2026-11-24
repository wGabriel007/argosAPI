using System.ComponentModel.DataAnnotations;

namespace argosAPI.Models;

/// <summary>Dados recebidos para criar ou atualizar uma autarquia.</summary>
public sealed class AutarquiaRequest
{
    public Guid Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string LegalName { get; set; } = string.Empty;
    [Required] public string Cnpj { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    [Required] public string Status { get; set; } = string.Empty;
}

/// <summary>Dados recebidos para criar ou atualizar um critério.</summary>
public sealed class CriterioRequest
{
    public Guid Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Type { get; set; } = string.Empty;
    [Required] public string Responsible { get; set; } = string.Empty;
    [Required] public string Status { get; set; } = string.Empty;
}

/// <summary>Dados recebidos para criar uma associação.</summary>
public sealed class AssociacaoRequest
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public Guid CriterionId { get; set; }
    [Required] public string Responsible { get; set; } = string.Empty;
}

/// <summary>Dados de uma etapa. Completed vazio significa não concluída.</summary>
public sealed class EtapaRequest
{
    public Guid Id { get; set; }
    public Guid AssociationId { get; set; }
    [Required] public string Process { get; set; } = string.Empty;
    [Required] public string Start { get; set; } = string.Empty;
    [Required] public string Due { get; set; } = string.Empty;
    public string Completed { get; set; } = string.Empty;
}

public sealed record ErrorResponse(string Message);
public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(string Name, string Email);

public sealed record MonitoramentoResponse(
    IReadOnlyList<object> Agencies,
    IReadOnlyList<object> Criteria,
    IReadOnlyList<object> Associations,
    IReadOnlyList<object> Stages);
