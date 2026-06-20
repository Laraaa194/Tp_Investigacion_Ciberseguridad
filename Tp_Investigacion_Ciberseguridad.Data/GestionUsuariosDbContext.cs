using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Data
{
    public class GestionUsuariosDbContext : IdentityDbContext<Usuario, Rol, string>
    {
        public GestionUsuariosDbContext(DbContextOptions<GestionUsuariosDbContext> options)
            : base(options)
        {
        }
        public DbSet<RegistroActividad> RegistrosActividad { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

        }
    }
}
