namespace argos.Domain.Entities;
/// <summary>
/// Entidade que representa Entidade Base do sistema
/// </summary>
public abstract class EntidadeBase
{
    /// <summary>
    /// Propriedade que representa o identificador unico
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Propriedade que representa Status
    /// </summary>
    public int Status { get; set; }
}
