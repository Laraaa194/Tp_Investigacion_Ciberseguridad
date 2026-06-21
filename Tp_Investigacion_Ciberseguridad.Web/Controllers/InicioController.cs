using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers
{

    [Authorize(Roles = "Usuario")]
    public class InicioController : Controller
    {
        [Route("Inicio")]
        public IActionResult Inicio()
        {
            return View();
        }
    }
}
