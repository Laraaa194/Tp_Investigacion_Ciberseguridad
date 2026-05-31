using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers
{
    public class AccountController : Controller
    {

        private readonly IUsuarioServicio _usuarioServicio;
        private readonly UserManager<Usuario> _userManager;

        public AccountController(IUsuarioServicio usuarioServicio, UserManager<Usuario> userManager)
        {
            _usuarioServicio = usuarioServicio;
            _userManager = userManager;
        }


        public IActionResult Registro()
        {
            return View();
        }

        public IActionResult InicioSesion()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrar(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Registro", model);

            var usuario = new Usuario
            {
                Email = model.Email,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                UserName = model.UserName,
                FechaNacimiento = model.FechaNacimiento
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);

            if (resultado.Succeeded)
                return RedirectToAction("InicioSesion");

            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View("Registro", model);
        }
    }
}
