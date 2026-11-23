namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa o Controle
/// </summary>
public class Controle : EntidadeBaseCadastro
{
    /// <summary>
    /// Propriedade que representa a referência da Autarquia
    /// </summary>
    public int AutarquiaId { get; set; }
    /// <summary>
    /// Propriedade que representa a referência do Usuario
    /// </summary>
    public int UsuarioId { get; set; }
    /// <summary>
    /// Propriedade que representa o processo Sei
    /// </summary>
    public string Sei { get; set; } = string.Empty;
    /// <summary>
    /// Propriedade que representa da data de início
    /// </summary>
    public DateTime DataInicio { get; set; }
    /// <summary>
    /// Propriedade que representa a data prevista
    /// </summary>
    public DateTime DataPrevista { get; set; }
    /// <summary>
    /// Propriedade que representa a data final
    /// </summary>
    public DateTime DataFinal { get; set; }
}
