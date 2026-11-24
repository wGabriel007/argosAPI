using argos.Domain.Enums;

namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa Criterios no sistema
/// </summary>
public class Criterio : EntidadeBaseCadastro
{
    /// <summary>
    /// Propriedade que representa Nome
    /// </summary>
    public string Nome {  get; set; } = string.Empty;
    /// <summary>
    /// Propriedade que representa Tipo
    /// </summary>
    public EnumTipoCriterio Tipo { get; set; }

    /// <summary>
    /// Propriedade que representa o responsavel.
    /// </summary>
    public string Responsavel { get; set; } = string.Empty;

    /// <summary>
    /// 0 = Inativa e 1 = Ativa.
    /// </summary>
    public EnumStatusCriterio Status { get; set; } = EnumStatusCriterio.Ativa;

    #region
    public ICollection<AutarquiaCriterio> Associacoes { get; set; } = new List<AutarquiaCriterio>();
    #endregion
}
