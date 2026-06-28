using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels;

namespace Tp_Investigacion_Ciberseguridad.Web.Mappers
{
    public static class UsuarioMapper
    {
        public static Usuario MapearAUsuario(RegistroViewModel model)
        {
            return new Usuario
            {
                UserName = model.UserName,
                Email = model.Email,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                FechaNacimiento = model.FechaNacimiento.Value,
                FechaRegistro = DateTime.UtcNow,
                EmailConfirmed = true
            };
        }
    }
}
