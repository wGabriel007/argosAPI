using argos.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace argosAPI.Controllers;

/// <summary>Retorna o conjunto completo usado pelo dashboard.</summary>
[ApiController]
[Route("monitoramento")]
[Authorize]
public sealed class MonitoramentoController(ArgosDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var agencies = await db.Autarquias.AsNoTracking().ToListAsync(cancellationToken);
        var criteria = await db.Criterios.AsNoTracking().ToListAsync(cancellationToken);
        var associations = await db.AutarquiaCriterios.AsNoTracking().ToListAsync(cancellationToken);
        var stages = await db.Controles.AsNoTracking().ToListAsync(cancellationToken);

        // O objeto é retornado diretamente, sem o envelope { data: ... }.
        return Ok(new
        {
            agencies = agencies.Select(x => new
            {
                id = x.Id,
                name = x.NomeFantasia,
                legalName = x.RazaoSocial,
                cnpj = x.Cnpj,
                email = x.Email,
                phone = x.Telefone,
                status = x.Status.ToString()
            }),
            criteria = criteria.Select(x => new
            {
                id = x.Id,
                name = x.Nome,
                type = x.Tipo.ToString(),
                responsible = x.Responsavel,
                status = x.Status.ToString()
            }),
            associations = associations.Select(x => new
            {
                id = x.Id,
                agencyId = x.AutarquiaId,
                criterionId = x.CriterioId,
                responsible = x.Responsavel
            }),
            stages = stages.Select(x => new
            {
                id = x.Id,
                associationId = x.AssociacaoId,
                process = x.Sei,
                start = x.DataInicio.ToString("yyyy-MM-dd"),
                due = x.DataPrevista.ToString("yyyy-MM-dd"),
                completed = x.DataFinal?.ToString("yyyy-MM-dd") ?? string.Empty
            })
        });
    }
}
