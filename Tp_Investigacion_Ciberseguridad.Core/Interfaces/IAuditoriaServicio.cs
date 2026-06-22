using System;
using System.Collections.Generic;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Dtos;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Core.Interfaces
{
    public interface IAuditoriaServicio
    {
        Task RegistrarAsync(string adminId, string adminNombre, TipoActividad tipo,
                             string usuarioAfectadoId, string usuarioAfectadoNombre, string? detalle = null);
        Task<List<RegistroActividad>> ObtenerUltimosAsync(int cantidad);

        Task<List<AlertaSeguridadDto>> ObtenerAlertasActividadAsync();
    }
}
