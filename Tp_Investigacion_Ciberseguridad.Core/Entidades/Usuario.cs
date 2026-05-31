using Microsoft.AspNetCore.Identity;

namespace Tp_Investigacion_Ciberseguridad.Core.Entidades
{
    public class Usuario : IdentityUser
    {

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }

        
        public DateTime FechaRegistro { get; set; }
        public bool UltimaConexionExitosa { get; set; }

        public Usuario() : base()
        {
            FechaRegistro = DateTime.Now;
        }
    }
}
