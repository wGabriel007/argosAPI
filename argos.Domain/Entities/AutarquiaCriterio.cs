namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa a tabela auxiliar
/// </summary>
public class AutarquiaCriterio : EntidadeBase
{
    /// <summary>
    /// Propriedade que representa a referência da Autarquia
    /// </summary>
    public int AutarquiaId { get; set; }
    /// <summary>
    /// Propriedade que representa a referência do Critério
    /// </summary>
    public int CriterioId { get; set; }

    public Autarquia? Autarquia { get; set; }
    public Criterio? Criterio { get; set; }
}
