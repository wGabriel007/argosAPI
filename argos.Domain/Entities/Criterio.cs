namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa Criterios no sistema
/// </summary>
public class Criterio : EntidadeBaseCadastro
{
    /// <summary>
    /// Propriedade que representa Nome
    /// </summary>
    public string Nome {  get; set; }
    /// <summary>
    /// Propriedade que representa Tipo
    /// </summary>
    public int Tipo { get; set; }
}
