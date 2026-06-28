using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Core.Interfaces
{
    public interface IUsuarioServicio
    {

        void guardarUsuario(Usuario usuario);

        Task<Usuario> ObtenerUsuarioPorEmail(string email);
        Task<Usuario> ObtenerUsuarioPorNombre(string nombre);
        Task<SignInResult> IniciarSesionAsync(Usuario usuario, string password, bool rememberMe);
        Task<SignInResult> ValidarCredencialesAsync(Usuario usuario, string password);
        Task<IdentityResult> GuardarUsuarioAsync(Usuario usuario, string password);
        Task<IdentityResult> AsignarRolAsync(Usuario usuario, string v);
        Task<IList<string>> ObtenerRolesDeUsuarioAsync(Usuario usuario);
        Task<bool> ExisteUsuarioAsync(string email);
        
        Task<Usuario> ObtenerUsuarioPorIdAsync(string id);
        Task<IdentityResult> ActualizarUsuarioAsync(Usuario usuario);
        Task<List<(Usuario Usuario, string Rol)>> BuscarUsuariosAsync(string q);
    }
}
