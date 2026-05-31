using System;
using System.Collections.Generic;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Data;

namespace Tp_Investigacion_Ciberseguridad.Core.Servicios
{
    public class UsuarioServicio : IUsuarioServicio
    {

        private readonly GestionUsuariosDbContext db;


        public UsuarioServicio(GestionUsuariosDbContext database) {

            db = database;
        }

        public void guardarUsuario(Usuario usuario)
        {
           
        }
    }
}
