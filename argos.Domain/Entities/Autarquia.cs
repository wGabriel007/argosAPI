using argos.Domain.Enums;

namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa Autarquias no sistema
/// </summary>
public class Autarquia : EntidadeBaseCadastro
{
    /// <summary>
    /// Propriedade que representa Cnpj
    /// </summary>
    public string Cnpj {  get; set; } = string.Empty;
    /// <summary>
    /// Propriedade que representa Nome Fantasia 
    /// </summary>
    public string NomeFantasia { get; set; } = string.Empty;
    /// <summary>
    /// Propriedade que representa Razão Social
    /// </summary>
    public string RazaoSocial { get; set; } = string.Empty;
    /// <summary>
    /// Propriedade que representa Telefone
    /// </summary>
    public string? Telefone {  get; set; }
    /// <summary>
    /// Propriedade que representa Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 0 = Pendente, 1 = Ativa e 2 = Inativa.
    /// </summary>
    public EnumStatusAutarquia Status { get; set; }

    /// <summary>
    /// Associações entre esta autarquia e seus critérios.
    /// </summary>
    public ICollection<AutarquiaCriterio> Associacoes { get; set; } = new List<AutarquiaCriterio>();
}
