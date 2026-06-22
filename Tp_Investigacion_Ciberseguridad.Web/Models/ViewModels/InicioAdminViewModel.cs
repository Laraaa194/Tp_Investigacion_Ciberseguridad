using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels
{
    public class InicioAdminViewModel
    {
        public int TotalUsuarios { get; set; }
        public int NuevosEstaSemana { get; set; }
        public List<UsuarioAdminViewModel> UltimosUsuarios { get; set; } = new();
        public List<RegistroActividadViewModel> ActividadReciente { get; set; } = new();
        public List<AlertaSeguridadViewModel> AlertasSeguridad { get; set; } = new();
    }
}
