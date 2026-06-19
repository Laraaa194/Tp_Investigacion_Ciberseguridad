using System;
using System.Collections.Generic;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Core.Interfaces
{
    public interface IAdminServicio
    {
        Task<List<Usuario>> ObtenerUsuariosAsync();

        Task<string> ObtenerRolAsync(Usuario usuario);

        Task EliminarUsuarioAsync(string id);
    }
}
