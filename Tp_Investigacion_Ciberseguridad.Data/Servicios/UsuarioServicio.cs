using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;

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

        public void guardarUsuario(Usuario usuario)
        {
           ;
        }

        public async Task<Usuario> ObtenerUsuarioPorEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
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

        public async Task<IdentityResult> GuardarUsuarioAsync(Usuario usuario, string password)
        {
            return await _signInManager.UserManager.CreateAsync(usuario, password);
        }

        public async Task<IdentityResult> AsignarRolAsync(Usuario usuario, string v)
        {
            return await _signInManager.UserManager.AddToRoleAsync(usuario, v);
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
    }
}
