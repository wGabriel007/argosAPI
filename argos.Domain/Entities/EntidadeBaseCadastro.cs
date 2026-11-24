namespace argos.Domain.Entities
{
    /// <summary>
    /// Entidade que representa Entidade Base de Cadastro
    /// </summary>
    public class EntidadeBaseCadastro : EntidadeBase
    {
        /// <summary>
        /// Propriedade que representa Data de cadastro
        /// </summary>
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Propriedade que representa Data de atualização
        /// </summary>
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
