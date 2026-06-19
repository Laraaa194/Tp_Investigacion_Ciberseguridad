using Microsoft.AspNetCore.Identity;
using System;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            return await _userManager.Users.ToListAsync();
        }

      

        public async Task EliminarUsuarioAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario != null)
            {
                await _userManager.DeleteAsync(usuario);
            }
        }
    }
}
