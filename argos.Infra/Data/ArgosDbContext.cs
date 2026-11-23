using argos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace argos.Infra.Data;

public class ArgosDbContext : DbContext
{
    public ArgosDbContext(DbContextOptions<ArgosDbContext> options) : base(options) { }

    public DbSet<Autarquia> Autarquia {  get; set; }
    public DbSet<Criterio> Criterio {  get; set; }
    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<AutarquiaCriterio> AutarquiaCriterio { get; set; }
    public DbSet<Controle> Controle { get; set; }
}
