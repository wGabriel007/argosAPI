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
    /// 1 = ativo; 0 = inativo.
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// Propriedade que representa a Senha.
    /// </summary>
    public string SenhaHash { get; set; } = string.Empty;
}
