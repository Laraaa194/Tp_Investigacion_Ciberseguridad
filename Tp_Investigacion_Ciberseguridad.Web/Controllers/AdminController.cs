using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult PanelAdmin()
        {
            return View();
        }
    }
}
