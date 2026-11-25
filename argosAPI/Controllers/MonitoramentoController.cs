using argos.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data.Common;

namespace argosAPI.Controllers;

/// <summary>Retorna o conjunto completo usado pelo dashboard.</summary>
[ApiController]
[Route("monitoramento")]
[Authorize]
public sealed class MonitoramentoController(
    ArgosDbContext db,
    ILogger<MonitoramentoController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var agencies     = await db.Autarquias.AsNoTracking().ToListAsync(cancellationToken);
            var criteria     = await db.Criterios.AsNoTracking().ToListAsync(cancellationToken);
            var associations = await db.AutarquiaCriterios.AsNoTracking().ToListAsync(cancellationToken);
            var stages       = await db.Controles.AsNoTracking().ToListAsync(cancellationToken);

            // O objeto é retornado diretamente, sem o envelope { data: ... }.
            return Ok(new
            {
                agencies = agencies.Select(x => new
                {
                    id        = x.Id,
                    name      = x.NomeFantasia,
                    legalName = x.RazaoSocial,
                    cnpj      = x.Cnpj,
                    email     = x.Email,
                    phone     = x.Telefone,
                    status    = x.Status.ToString()
                }),
                criteria = criteria.Select(x => new
                {
                    id          = x.Id,
                    name        = x.Nome,
                    type        = x.Tipo.ToString(),
                    responsible = x.Responsavel,
                    status      = x.Status.ToString()
                }),
                associations = associations.Select(x => new
                {
                    id          = x.Id,
                    agencyId    = x.AutarquiaId,
                    criterionId = x.CriterioId,
                    responsible = x.Responsavel
                }),
                stages = stages.Select(x => new
                {
                    // A tabela de controle guarda AutarquiaId, não AssociationId.
                    // Quando há uma única associação para a autarquia, conseguimos
                    // reconstruir o vínculo para manter compatibilidade com o frontend.
                    id = x.Id,
                    associationId = associations
                        .Where(a  => a.AutarquiaId == x.AutarquiaId)
                        .Select(a => (int?)a.Id)
                        .FirstOrDefault() ?? 0,
                    process       = x.Sei,
                    start         = x.DataInicio.ToString("yyyy-MM-dd"),
                    due           = x.DataPrevista.ToString("yyyy-MM-dd"),
                    completed     = x.DataFinal?.ToString("yyyy-MM-dd") ?? string.Empty
                })
            });
        }
        // O cliente encerrou ou cancelou a requisição. A exceção é relançada para
        // interromper corretamente a operação assíncrona sem retornar uma resposta
        // de erro para uma requisição que já não está mais sendo aguardada.
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Consulta de monitoramento cancelada pelo cliente.");
            throw;
        }
        // Falha de comunicação, indisponibilidade do banco ou erro SQL. O erro
        // detalhado é registrado no servidor, mas não é exposto ao cliente.
        catch (DbException exception)
        {
            logger.LogError(exception, "Falha de banco ao carregar o monitoramento.");

            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Serviço temporariamente indisponível.",
                detail: "Não foi possível consultar os dados do monitoramento.");
        }
        // O banco retornou um valor incompatível com o tipo definido no modelo,
        // como NULL em uma propriedade C# não anulável (por exemplo, int).
        catch (InvalidCastException exception)
        {
            logger.LogError(exception, "Falha ao materializar dados do monitoramento.");

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Dados inválidos.",
                detail: "Os dados armazenados não são compatíveis com o modelo da aplicação.");
        }
        // Proteção contra qualquer falha não prevista nas consultas ou na montagem
        // da resposta. O detalhe completo fica somente no log para evitar exposição
        // de informações internas da aplicação.
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro inesperado ao carregar o monitoramento.");

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro interno.",
                detail: "Não foi possível carregar o monitoramento.");
        }
    }
}
