using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class InicioController : Controller
    {
        private readonly UserManager<Usuario> _userManager;

        public InicioController(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Inicio()
        {
            var usuario = await _userManager.GetUserAsync(User);
            ViewBag.UserName = usuario?.Nombre ?? User.Identity?.Name;
            return View();
        }
    }
}