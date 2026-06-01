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
        private readonly SignInManager<Usuario> _signInManager;

        public AccountController(IUsuarioServicio usuarioServicio, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _usuarioServicio = usuarioServicio;
            _userManager = userManager;
            _signInManager = signInManager;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("InicioSesion", model);

            var usuario = await _userManager.FindByEmailAsync(model.Email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                return View("InicioSesion", model);
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                usuario.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true
            );

            if (resultado.Succeeded)
                return RedirectToAction("Inicio", "Inicio");

            if (resultado.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente. Intentá de nuevo más tarde.");
                return View("InicioSesion", model);
            }

            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
            return View("InicioSesion", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("InicioSesion", "Account");
        }


        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
