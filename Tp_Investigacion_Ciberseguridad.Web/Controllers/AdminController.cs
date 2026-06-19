using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Data.Servicios;
using Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminServicio _adminServicio;
        private readonly IUsuarioServicio _usuarioServicio;

        public AdminController(IAdminServicio adminServicio, IUsuarioServicio usuarioServicio)
        {
            _adminServicio = adminServicio;
            _usuarioServicio = usuarioServicio;
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
                await _usuarioServicio.AsignarRolAsync(usuario, model.Rol);
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
        public async Task<IActionResult> EliminarUsuario(string id) {

            if (id == null)
            {
                TempData["MensajeError"] = "ID de usuario inexistente.";
            }
            else { 
                await _adminServicio.EliminarUsuarioAsync(id);
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
                await _usuarioServicio.AsignarRolAsync(usuario, model.Rol);
                TempData["MensajeUsuarioExitoso"] = "Usuario actualizado exitosamente.";
                return RedirectToAction("PanelAdmin");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


    }
}
