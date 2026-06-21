using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
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
                .OrderByDescending(r => r.Fecha)
                .Take(cantidad)
                .ToListAsync();
        }
    }
}
