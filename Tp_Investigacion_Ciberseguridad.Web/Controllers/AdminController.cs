using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminServicio _adminServicio;
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IAuditoriaServicio _auditoriaServicio;

        public AdminController(IAdminServicio adminServicio, IUsuarioServicio usuarioServicio, IAuditoriaServicio auditoriaServicio)
        {
            _adminServicio = adminServicio;
            _usuarioServicio = usuarioServicio;
            _auditoriaServicio = auditoriaServicio;
        }

        public async Task<IActionResult> PanelAdmin()
        {
            var usuarios = await _adminServicio.ObtenerUsuariosAsync();
            var modelo = new List<UsuarioAdminViewModel>();

            foreach (var u in usuarios)
            {
                var rol = await _adminServicio.ObtenerRolAsync(u);
                modelo.Add(new UsuarioAdminViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Email = u.Email,
                    FechaRegistro = u.FechaRegistro,
                    Rol = rol
                });
            }
            return View(modelo);
        }


        public IActionResult NuevoUsuario()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoUsuarioAsync(NuevoUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _usuarioServicio.ExisteUsuarioAsync(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Ya existe un usuario con ese email.");
                return View(model);
            }

            Usuario usuario = new Usuario
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                UserName = model.UserName,
                Email = model.Email,
                FechaRegistro = DateTime.Now
            };

            var resultado = await _usuarioServicio.GuardarUsuarioAsync(usuario, model.Password);

            if (resultado.Succeeded)
            {

                var resultadoRol = await _usuarioServicio.AsignarRolAsync(usuario, model.Rol);
                if (!resultadoRol.Succeeded)
                {
                    foreach (var error in resultadoRol.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("NuevoUsuario", model);
                }

                await RegistrarActividadAsync(
                   tipo: TipoActividad.CreacionUsuario,
                   usuarioAfectadoId: usuario.Id,
                   usuarioAfectadoNombre: $"{usuario.Nombre} {usuario.Apellido}",
                   detalle: $"Rol asignado: {model.Rol}"
               );

                TempData["MensajeUsuarioExitoso"] = "Usuario creado exitosamente.";
                return RedirectToAction("PanelAdmin");
            }
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("NuevoUsuario", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(string id)
        {

            if (id == null)
            {
                TempData["MensajeError"] = "ID de usuario inexistente.";
            }
            else
            {
                Usuario usuario = await _usuarioServicio.ObtenerUsuarioPorIdAsync(id);
                await _adminServicio.EliminarUsuarioAsync(usuario.Id);

                await RegistrarActividadAsync(
                  tipo: TipoActividad.EliminacionUsuario,
                  usuarioAfectadoId: usuario.Id,
                  usuarioAfectadoNombre: $"{usuario.Nombre} {usuario.Apellido}"
              );
            }


            return RedirectToAction("PanelAdmin");
        }



        public async Task<IActionResult> EditarUsuario(string id)
        {
            var usuario = await _usuarioServicio.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                TempData["MensajeError"] = "Usuario no encontrado.";
                return RedirectToAction("PanelAdmin");
            }

            var modelo = new EditarUsuarioViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                UserName = usuario.UserName,
                Email = usuario.Email,
                Rol = await _adminServicio.ObtenerRolAsync(usuario)
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarUsuario(EditarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _usuarioServicio.ObtenerUsuarioPorIdAsync(model.Id);
            if (usuario == null)
            {
                TempData["MensajeError"] = "Usuario no encontrado.";
                return RedirectToAction("PanelAdmin");
            }

            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.UserName = model.UserName;
            usuario.Email = model.Email;

            var resultado = await _usuarioServicio.ActualizarUsuarioAsync(usuario);
            if (resultado.Succeeded)
            {
                var resultadoRol = await _usuarioServicio.AsignarRolAsync(usuario, model.Rol);
                if (!resultadoRol.Succeeded)
                {
                    foreach (var error in resultadoRol.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                await RegistrarActividadAsync(
                 tipo: TipoActividad.EdicionUsuario,
                 usuarioAfectadoId: usuario.Id,
                 usuarioAfectadoNombre: $"{usuario.Nombre} {usuario.Apellido}"
             );

                TempData["MensajeUsuarioExitoso"] = "Usuario actualizado exitosamente.";
                return RedirectToAction("PanelAdmin");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarUsuarios(string q)
        {
            var datos = await _usuarioServicio.BuscarUsuariosAsync(q);
            var resultado = datos.Select(d => new UsuarioAdminViewModel
            {
                Id = d.Usuario.Id,
                Nombre = d.Usuario.Nombre,
                Apellido = d.Usuario.Apellido,
                Email = d.Usuario.Email,
                Rol = d.Rol,
                FechaRegistro = d.Usuario.FechaRegistro
            });
            return Json(resultado);
        }

        [Route("Admin")]
        public async Task<IActionResult> Inicio()
        {
            var ultimosUsuarios = await _adminServicio.ObtenerUltimosRegistradosAsync(5);
            var modeloUltimos = new List<UsuarioAdminViewModel>();

            foreach (var u in ultimosUsuarios)
            {
                var rol = await _adminServicio.ObtenerRolAsync(u);
                modeloUltimos.Add(new UsuarioAdminViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Email = u.Email,
                    FechaRegistro = u.FechaRegistro,
                    Rol = rol
                });
            }

            var modelo = new InicioAdminViewModel
            {
                TotalUsuarios = await _adminServicio.ContarUsuariosAsync(),
                NuevosEstaSemana = await _adminServicio.ContarUsuariosNuevosAsync(7),
                UltimosUsuarios = modeloUltimos,
                ActividadReciente = await _auditoriaServicio.ObtenerUltimosAsync(5)
            };

            return View(modelo);
        }

        private async Task RegistrarActividadAsync(
            TipoActividad tipo,
            string usuarioAfectadoId,
            string usuarioAfectadoNombre,
            string? detalle = null)
                {
                    await _auditoriaServicio.RegistrarAsync(
                        adminId: User.FindFirstValue(ClaimTypes.NameIdentifier),
                        adminNombre: User.Identity?.Name ?? "Admin",
                        tipo: tipo,
                        usuarioAfectadoId: usuarioAfectadoId,
                        usuarioAfectadoNombre: usuarioAfectadoNombre,
                        detalle: detalle
            );
        }
    }
}
