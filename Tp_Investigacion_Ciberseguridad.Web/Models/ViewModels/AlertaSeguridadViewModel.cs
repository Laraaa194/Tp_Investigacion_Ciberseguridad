using Tp_Investigacion_Ciberseguridad.Core.Dtos;

namespace Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels
{
    public class AlertaSeguridadViewModel
    {
        public TipoAlerta Tipo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
