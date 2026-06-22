namespace Tp_Investigacion_Ciberseguridad.Core.Dtos
{
    public enum TipoAlerta
    {
        LoginFallido,
        CuentaBloqueada,
        NuevoRegistro
    }

    public class AlertaSeguridadDto
    {
        public TipoAlerta Tipo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}