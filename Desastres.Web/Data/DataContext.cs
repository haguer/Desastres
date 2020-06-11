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
    }
}
