using Microsoft.AspNetCore.Mvc;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Registro()
        {
            return View();
        }

        public IActionResult InicioSesion()
        {
            return View();
        }
    }
}
