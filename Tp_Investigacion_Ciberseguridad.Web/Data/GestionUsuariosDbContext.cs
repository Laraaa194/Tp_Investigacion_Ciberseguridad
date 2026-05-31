using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Tp_Investigacion_Ciberseguridad.Web.Models.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Web.Data
{
    public class GestionUsuariosDbContext : IdentityDbContext<Usuario, Rol, string>
    {
        public GestionUsuariosDbContext(DbContextOptions<GestionUsuariosDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

        }
    }
}
