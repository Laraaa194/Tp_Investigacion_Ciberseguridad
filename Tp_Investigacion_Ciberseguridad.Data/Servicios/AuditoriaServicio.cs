using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Dtos;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;

namespace Tp_Investigacion_Ciberseguridad.Data.Servicios
{
    public class AuditoriaServicio : IAuditoriaServicio
    {

        private readonly GestionUsuariosDbContext _context;

        public AuditoriaServicio(GestionUsuariosDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(string adminId, string adminNombre, TipoActividad tipo,
                                     string usuarioAfectadoId, string usuarioAfectadoNombre, string? detalle = null)
        {
            var registro = new RegistroActividad
            {
                AdminId = adminId,
                AdminNombre = adminNombre,
                Tipo = tipo,
                UsuarioAfectadoId = usuarioAfectadoId,
                UsuarioAfectadoNombre = usuarioAfectadoNombre,
                Detalle = detalle,
                Fecha = DateTime.Now
            };

            _context.RegistrosActividad.Add(registro);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RegistroActividad>> ObtenerUltimosAsync(int cantidad)
        {
            return await _context.RegistrosActividad
                .Where(r => r.Tipo == TipoActividad.CreacionUsuario ||
                            r.Tipo == TipoActividad.EdicionUsuario ||
                            r.Tipo == TipoActividad.EliminacionUsuario ||
                            r.Tipo == TipoActividad.CambioRol)
                .OrderByDescending(r => r.Fecha)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<List<AlertaSeguridadDto>> ObtenerAlertasActividadAsync()
        {
            var hoy = DateTime.Now.Date;
            var hace15Min = DateTime.Now.AddMinutes(-15);

            var registrosHoy = await _context.RegistrosActividad
                .Where(r => r.Fecha >= hoy &&
                    (r.Tipo == TipoActividad.LoginFallido || r.Tipo == TipoActividad.CuentaBloqueada))
                .ToListAsync();

            var alertas = new List<AlertaSeguridadDto>();


            var fallidosRecientes = registrosHoy
                .Where(r => r.Tipo == TipoActividad.LoginFallido && r.Fecha >= hace15Min)
                .GroupBy(r => r.UsuarioAfectadoId);

            foreach (var grupo in fallidosRecientes)
            {
                alertas.Add(new AlertaSeguridadDto
                {
                    Tipo = TipoAlerta.LoginFallido,
                    Titulo = $"{grupo.Count()} intento(s) de login fallidos",
                    Detalle = $"Cuenta: {grupo.First().UsuarioAfectadoNombre} — últimos 15 min",
                    Fecha = grupo.Max(r => r.Fecha)
                });
            }

            var bloqueosHoy = registrosHoy
                .Where(r => r.Tipo == TipoActividad.CuentaBloqueada);

            foreach (var r in bloqueosHoy)
            {
                alertas.Add(new AlertaSeguridadDto
                {
                    Tipo = TipoAlerta.CuentaBloqueada,
                    Titulo = "Cuenta bloqueada temporalmente",
                    Detalle = $"Usuario: {r.UsuarioAfectadoNombre} — {r.Detalle}",
                    Fecha = r.Fecha
                });
            }

            return alertas;
        }
    }
}
