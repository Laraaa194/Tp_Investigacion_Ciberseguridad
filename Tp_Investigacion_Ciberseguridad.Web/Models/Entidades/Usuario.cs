using Microsoft.AspNetCore.Identity;

namespace Tp_Investigacion_Ciberseguridad.Web.Models.Entidades
{
    public class Usuario : IdentityUser
    {
       
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        
        public DateTime FechaRegistro { get; set; }
        public bool UltimaConexionExitosa { get; set; }

        public Usuario() : base()
        {
            FechaRegistro = DateTime.UtcNow;
        }
    }
}
