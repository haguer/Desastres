using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Desastres.Web.Data.Entities;

namespace Desastres.Web.Data
{
    public class DataContext : IdentityDbContext<Usuario>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Administrador> Administradores{ get; set; }
        public DbSet<Encargado> Encargados { get; set; }
        public DbSet<Entidad> Entidades { get; set; }
        public DbSet<TipoDesastre> TipoDesastres { get; set; }
        public DbSet<Funcion> Funciones { get; set; }
        public DbSet<Emergencia> Emergencias { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Entidad>()
                .HasIndex(n => n.NombreEntidad)
                .IsUnique();

            builder.Entity<TipoDesastre>()
                .HasIndex(t => t.NombreDesastre).
                IsUnique();
            builder.Entity<Funcion>().HasKey(x => new { x.EntidadesId, x.TipoDesastresId });
        }
        public DbSet<Desastres.Web.Data.Entities.Funcion> Funcion { get; set; }
    }    

}
