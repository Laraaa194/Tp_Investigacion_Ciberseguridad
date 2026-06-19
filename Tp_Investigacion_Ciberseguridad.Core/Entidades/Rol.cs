using Microsoft.AspNetCore.Identity;

namespace Tp_Investigacion_Ciberseguridad.Core.Entidades
{
    public class Rol : IdentityRole
    {
        public string Descripcion { get; set; } = string.Empty;
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
