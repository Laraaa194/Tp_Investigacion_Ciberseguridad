using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers.Api
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosApiController : ControllerBase
    {
        [HttpGet("perfil")]
        public IActionResult GetPerfil()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            return Ok(new
            {
                mensaje = "Bienvenido a tu perfil.",
                usuarioEmail = email
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("panel-admin")]
        public IActionResult GetDatosAdmin()
        {
            return Ok(new
            {
                mensaje = "Acceso de administrador confirmado. Acá irían los datos restringidos."
            });
        }
    }
}