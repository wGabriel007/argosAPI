namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa o Controle
/// </summary>
public class Controle : EntidadeBaseCadastro
{
    /// <summary>
    /// Propriedade que representa a referência da Autarquia
    /// </summary>
    /// <summary>
    /// Associação à qual esta etapa pertence.
    /// </summary>
    public Guid AssociacaoId { get; set; }
    /// <summary>
    /// Propriedade que representa o processo Sei
    /// </summary>
    public string Sei { get; set; } = string.Empty;
    /// <summary>
    /// Propriedade que representa da data de início
    /// </summary>
    public DateOnly DataInicio { get; set; }
    /// <summary>
    /// Propriedade que representa a data prevista
    /// </summary>
    public DateOnly DataPrevista { get; set; }
    /// <summary>
    /// Propriedade que representa a data final
    /// </summary>
    public DateOnly? DataFinal { get; set; }

    public AutarquiaCriterio Associacao { get; set; } = null!;
}
