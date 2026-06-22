using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using Tp_Investigacion_Ciberseguridad.Core.Dtos;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;

namespace Tp_Investigacion_Ciberseguridad.Data.Servicios
{
    public class AdminServicio : IAdminServicio
    {

        private readonly UserManager<Usuario> _userManager;

        public AdminServicio(UserManager<Usuario> userManager) { 
            _userManager = userManager;
        }

        public async Task<string> ObtenerRolAsync(Usuario usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            return roles.FirstOrDefault() ?? "Sin rol";
        }

        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            return await _userManager.Users
                .OrderBy(u => u.Apellido)
                .ThenBy(u => u.Nombre)
                .ToListAsync();
        }



        public async Task EliminarUsuarioAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario != null)
            {
                await _userManager.DeleteAsync(usuario);
            }
        }

        public async Task<int> ContarUsuariosAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<int> ContarUsuariosNuevosAsync(int dias)
        {
            var fechaLimite = DateTime.Now.AddDays(-dias);
            return await _userManager.Users
                .Where(u => u.FechaRegistro >= fechaLimite)
                .CountAsync();
        }

        public async Task<List<Usuario>> ObtenerUltimosRegistradosAsync(int cantidad)
        {
            return await _userManager.Users
                .OrderByDescending(u => u.FechaRegistro)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<List<AlertaSeguridadDto>> ObtenerAlertasUsuariosAsync()
        {
            var alertas = new List<AlertaSeguridadDto>();
            var hoy = DateTime.Now.Date;


            var nuevosHoy = await _userManager.Users
                .Where(u => u.FechaRegistro.Date == hoy)
                .CountAsync();

            if (nuevosHoy > 0)
            {
                alertas.Add(new AlertaSeguridadDto
                {
                    Tipo = TipoAlerta.NuevoRegistro,
                    Titulo = $"{nuevosHoy} usuario(s) nuevos hoy",
                    Detalle = "Registrados en el día de la fecha",
                    Fecha = DateTime.Now
                });
            }

            return alertas;
        }
    }
}
