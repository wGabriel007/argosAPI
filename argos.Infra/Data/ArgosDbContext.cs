using argos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace argos.Infra.Data;

public class ArgosDbContext : DbContext
{
    public ArgosDbContext(DbContextOptions<ArgosDbContext> options) : base(options) { }

    public DbSet<Autarquia> Autarquias => Set<Autarquia>();
    public DbSet<Criterio> Criterios => Set<Criterio>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<AutarquiaCriterio> AutarquiaCriterios => Set<AutarquiaCriterio>();
    public DbSet<Controle> Controles => Set<Controle>();

    /// <summary>
    /// Centraliza nomes de tabelas, índices e relacionamentos para que o modelo
    /// do EF Core permaneça igual ao banco PostgreSQL.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Autarquia>(entity =>
        {
            entity.ToTable("tb_autarquia");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Cnpj).IsUnique();
            entity.Property(x => x.Id).HasColumnName("iId").ValueGeneratedOnAdd();
            entity.Property(x => x.Cnpj).HasColumnName("sCnpj").IsRequired().HasMaxLength(18);
            entity.Property(x => x.NomeFantasia).HasColumnName("sNomeFantasia").IsRequired().HasMaxLength(150);
            entity.Property(x => x.RazaoSocial).HasColumnName("sRazaoSocial").IsRequired().HasMaxLength(250);
            entity.Property(x => x.Telefone).HasColumnName("sTelefone");
            entity.Property(x => x.Email).HasColumnName("sEmail");
            entity.Property(x => x.Status).HasColumnName("iStatus");
            entity.Property(x => x.DataCadastro).HasColumnName("dtDataCadastro");
            entity.Property(x => x.DataAtualizacao).HasColumnName("dtDataAtualizacao");
        });

        modelBuilder.Entity<Criterio>(entity =>
        {
            entity.ToTable("tb_criterio");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("iId").ValueGeneratedOnAdd();
            entity.Property(x => x.Nome).HasColumnName("sNome").IsRequired().HasMaxLength(150);
            entity.Property(x => x.Tipo).HasColumnName("iTipo");
            entity.Property(x => x.Responsavel).HasColumnName("sResponsavel").IsRequired().HasMaxLength(150);
            entity.Property(x => x.Status).HasColumnName("iStatus");
            entity.Property(x => x.DataCadastro).HasColumnName("dtDataCadastro");
            entity.Property(x => x.DataAtualizacao).HasColumnName("dtDataAtualizacao");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("tb_usuario");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Id).HasColumnName("iId").ValueGeneratedOnAdd();
            entity.Property(x => x.Nome).HasColumnName("sNome").IsRequired().HasMaxLength(150);
            entity.Property(x => x.Email).HasColumnName("sEmail").IsRequired().HasMaxLength(254);
            entity.Property(x => x.Perfil).HasColumnName("iPerfil");
            entity.Property(x => x.Status).HasColumnName("iStatus");
            entity.Property(x => x.SenhaHash).HasColumnName("sSenha").IsRequired();
            entity.Property(x => x.DataCadastro).HasColumnName("dtDataCadastro");
            entity.Property(x => x.DataAtualizacao).HasColumnName("dtDataAtualizacao");
        });

        modelBuilder.Entity<AutarquiaCriterio>(entity =>
        {
            entity.ToTable("tb_autarquia_criterio");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AutarquiaId, x.CriterioId }).IsUnique();
            entity.Property(x => x.Id).HasColumnName("iId").ValueGeneratedOnAdd();
            entity.Property(x => x.AutarquiaId).HasColumnName("iAutarquiaId");
            entity.Property(x => x.CriterioId).HasColumnName("iCriterioId");
            entity.Property(x => x.Responsavel).HasColumnName("sResponsavel").IsRequired().HasMaxLength(150);

            // A tabela tb_autarquia_criterio existente não possui colunas de
            // auditoria. Essas propriedades vêm da classe base, mas não devem
            // participar do INSERT/UPDATE dessa tabela.
            entity.Ignore(x => x.DataCadastro);
            entity.Ignore(x => x.DataAtualizacao);

            entity.HasOne(x => x.Autarquia)
                .WithMany(x => x.Associacoes)
                .HasForeignKey(x => x.AutarquiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Criterio)
                .WithMany(x => x.Associacoes)
                .HasForeignKey(x => x.CriterioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Controle>(entity =>
        {
            entity.ToTable("tb_controle");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AssociacaoId).IsUnique();
            entity.Property(x => x.Id).HasColumnName("iId").ValueGeneratedOnAdd();
            entity.Property(x => x.AssociacaoId).HasColumnName("iAssociacaoId");
            entity.Property(x => x.Sei).HasColumnName("sSei").IsRequired().HasMaxLength(25);
            entity.Property(x => x.DataInicio).HasColumnName("dtDataInicio");
            entity.Property(x => x.DataPrevista).HasColumnName("dtDataFinalPrevista");
            entity.Property(x => x.DataFinal).HasColumnName("dtDataFinal");
            entity.Property(x => x.DataCadastro).HasColumnName("dtDataCadastro");
            entity.Property(x => x.DataAtualizacao).HasColumnName("dtDataAtualizacao");

            entity.HasOne(x => x.Associacao)
                .WithOne(x => x.Controle)
                .HasForeignKey<Controle>(x => x.AssociacaoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
