using Microsoft.AspNetCore.Identity;

namespace Tp_Investigacion_Ciberseguridad.Web.Models.Entidades
{
    public class Rol : IdentityRole
    {
        public string Descripcion { get; set; }
        public int NivelJerarquia { get; set; } 
        public DateTime FechaCreacion { get; set; }
        public bool EstaActivo { get; set; }

        
        public Rol() : base()
        {
            FechaCreacion = DateTime.UtcNow;
            EstaActivo = true;
        }

       
        public Rol(string roleName, string descripcion, int nivelJerarquia) : base(roleName)
        {
            Descripcion = descripcion;
            NivelJerarquia = nivelJerarquia;
            FechaCreacion = DateTime.UtcNow;
            EstaActivo = true;
        }
    }
}
