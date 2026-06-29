using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tp_Investigacion_Ciberseguridad.Data.Servicios
{
    public class UsuarioServicio : IUsuarioServicio
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public UsuarioServicio(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<Usuario> ObtenerUsuarioPorEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Usuario> ObtenerUsuarioPorNombre(string nombre)
        {
            return await _userManager.FindByNameAsync(nombre);
        }

        public async Task<SignInResult> IniciarSesionAsync(Usuario usuario, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(
                usuario.UserName,
                password,
                rememberMe,
                lockoutOnFailure: true
            );
        }

        public async Task<SignInResult> ValidarCredencialesAsync(Usuario usuario, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(usuario, password, lockoutOnFailure: true);
        }

        public async Task<IdentityResult> GuardarUsuarioAsync(Usuario usuario, string password)
        {
            return await _userManager.CreateAsync(usuario, password);
        }

        public async Task<IdentityResult> AsignarRolAsync(Usuario usuario, string nuevoRol)
        {
            var rolesActuales = await _userManager.GetRolesAsync(usuario);

            if (rolesActuales.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
                if (!removeResult.Succeeded)
                {
                    return removeResult;
                }
            }
            return await _userManager.AddToRoleAsync(usuario, nuevoRol);
        }

        public async Task<IList<string>> ObtenerRolesDeUsuarioAsync(Usuario usuario)
        {
            return await _userManager.GetRolesAsync(usuario);
        }

        public Task<bool> ExisteUsuarioAsync(string email)
        {
            Usuario usuario = ObtenerUsuarioPorEmail(email).Result;

            if (usuario != null) { 
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<Usuario> ObtenerUsuarioPorIdAsync(string id)
        {
            var Usuario =  _userManager.FindByIdAsync(id);
            return Usuario;
        }

        public async Task<IdentityResult> ActualizarUsuarioAsync(Usuario usuario)
        {
            return await _userManager.UpdateAsync(usuario);
        }

        public async Task<List<(Usuario Usuario, string Rol)>> BuscarUsuariosAsync(string q)
        {
            string query = q?.Trim() ?? string.Empty;

            var usuarios = await _userManager.Users
                .Where(u => string.IsNullOrEmpty(query) ||
                            u.Nombre.Contains(query) ||
                            u.Apellido.Contains(query) ||
                            u.Email.Contains(query))
                .ToListAsync();

            var resultado = new List<(Usuario Usuario, string Rol)>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                resultado.Add((usuario, roles.FirstOrDefault() ?? string.Empty));
            }

            return resultado;
        }
    }
}
