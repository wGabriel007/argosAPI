namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa Usuario no sistema
/// </summary>
public class Usuario : EntidadeBaseCadastro
{
    /// <summary>
    /// Propriedade que representa o Nome
    /// </summary>
    public string Nome { get; set; } = string.Empty;
    /// <summary>
    /// Propriedade que representa o Email
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Propriedade que representa o Perfil
    /// </summary>
    public int Perfil {  get; set; }
    /// <summary>
    /// Propriedade que representa a Senha
    /// </summary>
    public string Senha { get; set; } = string.Empty;
}
