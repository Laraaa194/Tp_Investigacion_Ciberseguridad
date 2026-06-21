using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

            var usuario = await _usuarioServicio.ObtenerUsuarioPorEmail(model.Email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                return View("InicioSesion", model);
            }

            var resultado = await _usuarioServicio.IniciarSesionAsync(usuario, model.Password, model.RememberMe);

            if (resultado.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(usuario, "Admin"))
                {
                    return RedirectToAction("Inicio", "Admin");
                }
                return RedirectToAction("Inicio", "Inicio");
            }

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

            var resultado = await _usuarioServicio.GuardarUsuarioAsync(usuario, model.Password);

            if (resultado.Succeeded)
            {
                await _usuarioServicio.AsignarRolAsync(usuario, "Usuario");
                return RedirectToAction("InicioSesion");
            }
            foreach (var error in resultado.Errors) { 
            ModelState.AddModelError(string.Empty, error.Description);
            }
                

            return View("Registro", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginExterno(string proveedor)
        {
            // 1. Configuramos a dónde tiene que volver el usuario una vez que Google lo autentique.
            // Apuntamos a una acción de tu controlador que procesará el resultado.
            var urlRetorno = Url.Action("CallbackLoginExterno", "Account");

            // 2. Le pedimos a Identity que arme las propiedades del desafío de OAuth 2.0
            var propiedades = _signInManager.ConfigureExternalAuthenticationProperties(proveedor, urlRetorno);

            // 3. Lanzamos el desafío (ChallengeResult). 
            // Esto le dice a ASP.NET: "Tomá el control vos y redirigí al usuario a Google de forma segura".
            return Challenge(propiedades, proveedor);
        }

        // Esta es la acción que va a recibir la respuesta final de Google
        [HttpGet]
        public async Task<IActionResult> CallbackLoginExterno(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Action("Inicio", "Inicio");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error del proveedor externo: {remoteError}");
                return View("InicioSesion");
            }

            // Obtenemos la información del usuario que nos devuelve Google
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("InicioSesion");
            }

            // Intentamos iniciar sesión con los datos de Google
            var resultado = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (resultado.Succeeded)
            {
                // El usuario ya existía y se logueó con éxito
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Si el usuario no existe en tu base de datos, lo creamos automáticamente


                var email = info.Principal.FindFirstValue(ClaimTypes.Email);


                if (email != null)
                {
                    var nombreGoogle = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                    var apellidoGoogle = info.Principal.FindFirstValue(ClaimTypes.Surname);
                    var nuevoUsuario = new Usuario
                    {
                        UserName = email,
                        Email = email,
                        Nombre = nombreGoogle ?? "Usuario",
                        Apellido = apellidoGoogle ?? "Google"
                    };

                    // Acá podés mapear campos extra de tu entidad si los necesitás

                    var resultadoCreacion = await _userManager.CreateAsync(nuevoUsuario);
                    if (resultadoCreacion.Succeeded)
                    {
                        // Asociamos la cuenta de Google con el usuario que acabamos de crear
                        resultadoCreacion = await _userManager.AddLoginAsync(nuevoUsuario, info);
                        if (resultadoCreacion.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(nuevoUsuario, "Usuario");
                            await _signInManager.SignInAsync(nuevoUsuario, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                }

                return View("InicioSesion");
            }
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
