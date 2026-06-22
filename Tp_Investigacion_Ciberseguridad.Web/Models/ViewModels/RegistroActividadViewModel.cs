using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels
{
    public class RegistroActividadViewModel
    {
        public int Id { get; set; }
        public string AdminNombre { get; set; } = string.Empty;
        public TipoActividad Tipo { get; set; }
        public string UsuarioAfectadoNombre { get; set; } = string.Empty;
        public string? Detalle { get; set; }
        public DateTime Fecha { get; set; }
    }
}