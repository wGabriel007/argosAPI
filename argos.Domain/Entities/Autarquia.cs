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
}
