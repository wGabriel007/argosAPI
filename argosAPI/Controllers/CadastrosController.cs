using System.Security.Claims;
using argos.Domain.Entities;
using argos.Domain.Enums;
using argosAPI.Helpers;
using argosAPI.Models;
using argos.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace argosAPI.Controllers;

/// <summary>Endpoints de autarquias, critérios, associações e etapas.</summary>
[ApiController]
[Route("")]
[Authorize]
public sealed class CadastrosController(ArgosDbContext db) : ControllerBase
{
    [HttpPost("autarquias")]
    public async Task<IActionResult> CreateAgency(AutarquiaRequest request, CancellationToken ct)
    {
        if (!ApiRules.IsValidCnpj(request.Cnpj)) return BadRequest(new ErrorResponse("CNPJ inválido."));
        if (!Enum.TryParse<EnumStatusAutarquia>(request.Status, true, out var status)) return BadRequest(new ErrorResponse("Status de autarquia inválido."));
        var cnpj = ApiRules.NormalizeCnpj(request.Cnpj);
        if (await db.Autarquias.AnyAsync(x => x.Cnpj == cnpj, ct)) return Conflict(new ErrorResponse("CNPJ já cadastrado."));

        // O ID é gerado pelo identity do banco; o valor enviado no POST é ignorado.
        var entity = new Autarquia { Cnpj = cnpj, NomeFantasia = request.Name, RazaoSocial = request.LegalName, Email = request.Email, Telefone = request.Phone, Status = status };
        db.Autarquias.Add(entity);
        await db.SaveChangesAsync(ct);
        return Created($"/autarquias/{entity.Id}", Agency(entity));
    }

    [HttpPut("autarquias/{id:int}")]
    public async Task<IActionResult> UpdateAgency(int id, AutarquiaRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest(new ErrorResponse("O id da URL deve ser igual ao id do corpo."));
        if (!ApiRules.IsValidCnpj(request.Cnpj)) return BadRequest(new ErrorResponse("CNPJ inválido."));
        if (!Enum.TryParse<EnumStatusAutarquia>(request.Status, true, out var status)) return BadRequest(new ErrorResponse("Status de autarquia inválido."));
        var entity = await db.Autarquias.FindAsync([id], ct);
        if (entity is null) return NotFound(new ErrorResponse("Autarquia não encontrada."));
        var cnpj = ApiRules.NormalizeCnpj(request.Cnpj);
        if (await db.Autarquias.AnyAsync(x => x.Id != id && x.Cnpj == cnpj, ct)) return Conflict(new ErrorResponse("CNPJ já cadastrado."));
        entity.Cnpj = cnpj; entity.NomeFantasia = request.Name; entity.RazaoSocial = request.LegalName; entity.Email = request.Email; entity.Telefone = request.Phone; entity.Status = status; entity.DataAtualizacao = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return Ok(Agency(entity));
    }

    [HttpPost("criterios")]
    public async Task<IActionResult> CreateCriterion(CriterioRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<EnumTipoCriterio>(request.Type, true, out var type) || !Enum.TryParse<EnumStatusCriterio>(request.Status, true, out var status)) return BadRequest(new ErrorResponse("Tipo ou status de critério inválido."));
        // O ID é gerado pelo identity do banco; o valor enviado no POST é ignorado.
        var entity = new Criterio { Nome = request.Name, Tipo = type, Responsavel = request.Responsible, Status = status };
        db.Criterios.Add(entity); await db.SaveChangesAsync(ct); return Created($"/criterios/{entity.Id}", Criterion(entity));
    }

    [HttpPut("criterios/{id:int}")]
    public async Task<IActionResult> UpdateCriterion(int id, CriterioRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest(new ErrorResponse("O id da URL deve ser igual ao id do corpo."));
        if (!Enum.TryParse<EnumTipoCriterio>(request.Type, true, out var type) || !Enum.TryParse<EnumStatusCriterio>(request.Status, true, out var status)) return BadRequest(new ErrorResponse("Tipo ou status de critério inválido."));
        var entity = await db.Criterios.FindAsync([id], ct); if (entity is null) return NotFound(new ErrorResponse("Critério não encontrado."));
        entity.Nome = request.Name; entity.Tipo = type; entity.Responsavel = request.Responsible; entity.Status = status; entity.DataAtualizacao = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return Ok(Criterion(entity));
    }

    [HttpPost("associacoes")]
    public async Task<IActionResult> CreateAssociation(AssociacaoRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Responsible)) return BadRequest(new ErrorResponse("O responsável é obrigatório."));
        var agency = await db.Autarquias.FindAsync([request.AgencyId], ct); var criterion = await db.Criterios.FindAsync([request.CriterionId], ct);
        if (agency is null || criterion is null) return NotFound(new ErrorResponse("Autarquia ou critério não encontrado."));
        if (agency.Status != EnumStatusAutarquia.Ativa || criterion.Status != EnumStatusCriterio.Ativa) return Conflict(new ErrorResponse("Autarquia e critério precisam estar ativos."));
        if (await db.AutarquiaCriterios.AnyAsync(x => x.AutarquiaId == request.AgencyId && x.CriterioId == request.CriterionId, ct)) return Conflict(new ErrorResponse("Este critério já está associado à autarquia."));
        // O ID é gerado pelo identity do banco; o valor enviado no POST é ignorado.
        var entity = new AutarquiaCriterio { AutarquiaId = request.AgencyId, CriterioId = request.CriterionId, Responsavel = request.Responsible };
        db.AutarquiaCriterios.Add(entity); await db.SaveChangesAsync(ct); return Created($"/associacoes/{entity.Id}", Association(entity));
    }

    [HttpDelete("associacoes/{id:int}")]
    public async Task<IActionResult> DeleteAssociation(int id, CancellationToken ct)
    {
        var entity = await db.AutarquiaCriterios.FindAsync([id], ct); if (entity is null) return NotFound(new ErrorResponse("Associação não encontrada."));
        db.AutarquiaCriterios.Remove(entity); await db.SaveChangesAsync(ct); return NoContent();
    }

    [HttpPost("etapas")]
    public async Task<IActionResult> CreateStage(EtapaRequest request, CancellationToken ct)
    {
        var result = await ValidateStage(request, ct); if (result.Error is not null) return result.Error;
        var association = await db.AutarquiaCriterios.AsNoTracking().SingleAsync(x => x.Id == request.AssociationId, ct);
        var userId = CurrentUserId();
        if (userId is null) return Unauthorized(new ErrorResponse("Usuário autenticado inválido."));
        if (await db.Controles.AnyAsync(x => x.AutarquiaId == association.AutarquiaId, ct)) return Conflict(new ErrorResponse("A autarquia já possui uma etapa."));
        var entity = ToStage(request, association.AutarquiaId, userId.Value);
        db.Controles.Add(entity); await db.SaveChangesAsync(ct); return Created($"/etapas/{entity.Id}", Stage(entity, request.AssociationId));
    }

    [HttpPut("etapas/{id:int}")]
    public async Task<IActionResult> UpdateStage(int id, EtapaRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest(new ErrorResponse("O id da URL deve ser igual ao id do corpo."));
        var result = await ValidateStage(request, ct); if (result.Error is not null) return result.Error;
        var entity = await db.Controles.FindAsync([id], ct); if (entity is null) return NotFound(new ErrorResponse("Etapa não encontrada."));
        var association = await db.AutarquiaCriterios.AsNoTracking().SingleAsync(x => x.Id == request.AssociationId, ct);
        var userId = CurrentUserId();
        if (userId is null) return Unauthorized(new ErrorResponse("Usuário autenticado inválido."));
        if (await db.Controles.AnyAsync(x => x.Id != id && x.AutarquiaId == association.AutarquiaId, ct)) return Conflict(new ErrorResponse("A autarquia já possui uma etapa."));
        entity.AutarquiaId = association.AutarquiaId; entity.UsuarioId = userId.Value; entity.Sei = request.Process; entity.DataInicio = result.Start; entity.DataPrevista = result.Due; entity.DataFinal = result.Completed; entity.DataAtualizacao = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return Ok(Stage(entity, request.AssociationId));
    }

    private async Task<(IActionResult? Error, DateOnly Start, DateOnly Due, DateOnly? Completed)> ValidateStage(EtapaRequest request, CancellationToken ct)
    {
        if (!ApiRules.IsValidProcess(request.Process) || !ApiRules.TryDate(request.Start, out var start) || !ApiRules.TryDate(request.Due, out var due)) return (BadRequest(new ErrorResponse("Processo ou data inválida.")), default, default, null);
        if (due < start) return (BadRequest(new ErrorResponse("A data final prevista não pode ser anterior ao início.")), default, default, null);
        if (!string.IsNullOrWhiteSpace(request.Completed) && (!ApiRules.TryDate(request.Completed, out var completed) || completed < start || completed > ApiRules.TodayBrasilia())) return (BadRequest(new ErrorResponse("Data de conclusão inválida.")), default, default, null);
        if (!await db.AutarquiaCriterios.AnyAsync(x => x.Id == request.AssociationId, ct)) return (NotFound(new ErrorResponse("Associação não encontrada.")), default, default, null);
        DateOnly? final = string.IsNullOrWhiteSpace(request.Completed) ? null : DateOnly.Parse(request.Completed);
        return (null, start, due, final);
    }
    #region
    private int? CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    private static Controle ToStage(EtapaRequest x, int autarquiaId, int usuarioId) => new() { AutarquiaId = autarquiaId, UsuarioId = usuarioId, Sei = x.Process, DataInicio = DateOnly.Parse(x.Start), DataPrevista = DateOnly.Parse(x.Due), DataFinal = string.IsNullOrWhiteSpace(x.Completed) ? null : DateOnly.Parse(x.Completed) };
    private static object Agency(Autarquia x)              => new { id = x.Id, name = x.NomeFantasia, legalName = x.RazaoSocial, cnpj = x.Cnpj, email = x.Email, phone = x.Telefone, status = x.Status.ToString() };
    private static object Criterion(Criterio x)            => new { id = x.Id, name = x.Nome, type = x.Tipo.ToString(), responsible = x.Responsavel, status = x.Status.ToString() };
    private static object Association(AutarquiaCriterio x) => new { id = x.Id, agencyId = x.AutarquiaId, criterionId = x.CriterioId, responsible = x.Responsavel };
    private static object Stage(Controle x, int associationId) => new { id = x.Id, associationId, process = x.Sei, start = x.DataInicio.ToString("yyyy-MM-dd"), due = x.DataPrevista.ToString("yyyy-MM-dd"), completed = x.DataFinal?.ToString("yyyy-MM-dd") ?? string.Empty };
    #endregion
}
