using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers
{
    [Authorize]
    public class PerfilController : Controller

    {
        private readonly UserManager<Usuario> _userManager;

        public PerfilController(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        [Route("Perfil")]
        public async Task<IActionResult> PerfilAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);


            var model = new PerfilViewModel
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                UserName = usuario.UserName,
                Email = usuario.Email,
                FechaNacimiento = usuario.FechaNacimiento,
            };

            return View(model);
        }
    }
}
