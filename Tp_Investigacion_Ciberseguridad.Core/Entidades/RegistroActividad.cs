using System;
using System.Collections.Generic;
using System.Text;

namespace Tp_Investigacion_Ciberseguridad.Core.Entidades
{
    public class RegistroActividad
    {
    
            public int Id { get; set; }
            public string AdminId { get; set; } = string.Empty;       
            public string AdminNombre { get; set; } = string.Empty;    
            public TipoActividad Tipo { get; set; }
            public string UsuarioAfectadoId { get; set; } = string.Empty;
            public string UsuarioAfectadoNombre { get; set; } = string.Empty;
            public string? Detalle { get; set; }                       
            public DateTime Fecha { get; set; }
     }

    public enum TipoActividad
    {
        CreacionUsuario,
        EdicionUsuario,
        EliminacionUsuario,
        CambioRol,
        LoginFallido,
        CuentaBloqueada
    }
    }

