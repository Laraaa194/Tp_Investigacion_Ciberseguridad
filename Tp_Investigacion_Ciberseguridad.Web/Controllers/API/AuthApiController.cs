using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Web.Mappers;
using Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IAuditoriaServicio _auditoriaServicio;
        private readonly ITokenServicio _tokenServicio;

        public AuthApiController(IUsuarioServicio usuarioServicio, IAuditoriaServicio auditoriaServicio, ITokenServicio tokenServicio)
        {
            _usuarioServicio = usuarioServicio;
            _auditoriaServicio = auditoriaServicio;
            _tokenServicio = tokenServicio;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _usuarioServicio.ObtenerUsuarioPorEmail(model.Email);

            if (usuario == null)
            {
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            var resultado = await _usuarioServicio.ValidarCredencialesAsync(usuario, model.Password);

            if (resultado.IsLockedOut)
            {
                await _auditoriaServicio.RegistrarAsync(
                    adminId: "sistema-api",
                    adminNombre: "Sistema API",
                    tipo: TipoActividad.CuentaBloqueada,
                    usuarioAfectadoId: usuario.Id,
                    usuarioAfectadoNombre: usuario.Email ?? usuario.UserName ?? "Desconocido",
                    detalle: "Se supero el limite de intentos fallidos"
                );
                return Unauthorized(new { mensaje = "Cuenta bloqueada temporalmente. Intentar de nuevo mas tarde." });
            }

            if (!resultado.Succeeded)
            {
                await _auditoriaServicio.RegistrarAsync(
                    adminId: "sistema-api",
                    adminNombre: "Sistema API",
                    tipo: TipoActividad.LoginFallido,
                    usuarioAfectadoId: usuario.Id,
                    usuarioAfectadoNombre: usuario.Email ?? usuario.UserName ?? "Desconocido",
                    detalle: "Intento de login fallido"
                );

                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            var roles = await _usuarioServicio.ObtenerRolesDeUsuarioAsync(usuario);

            var jwt = _tokenServicio.GenerarToken(usuario, roles);

            return Ok(new
            {
                mensaje = "Login exitoso",
                token = jwt.TokenString,
                expiracion = jwt.Expiracion
            });
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailExistente = await _usuarioServicio.ObtenerUsuarioPorEmail(model.Email);
            if (emailExistente != null)
            {
                return BadRequest(new { mensaje = "El correo electronico ya registrado." });
            }

            var userExistente = await _usuarioServicio.ObtenerUsuarioPorNombre(model.UserName);
            if (userExistente != null)
            {
                return BadRequest(new { mensaje = "Nombre de usuario existente." });
            }

            Usuario nuevoUsuario = UsuarioMapper.MapearAUsuario(model);

            var resultado = await _usuarioServicio.GuardarUsuarioAsync(nuevoUsuario, model.Password);

            if (!resultado.Succeeded)
            {
                return BadRequest(new { errores = resultado.Errors.Select(e => e.Description) });
            }

            await _usuarioServicio.AsignarRolAsync(nuevoUsuario, "Usuario");

            return Ok(new { mensaje = "Usuario registrado con exito" });
        }
    }
}