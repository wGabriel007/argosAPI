namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa o Controle
/// </summary>
public class Controle : EntidadeBaseCadastro
{
    /// <summary>Autarquia responsável pelo controle.</summary>
    public int AutarquiaId { get; set; }

    /// <summary>Usuário responsável pelo controle.</summary>
    public int UsuarioId { get; set; }
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

    public Autarquia Autarquia { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
