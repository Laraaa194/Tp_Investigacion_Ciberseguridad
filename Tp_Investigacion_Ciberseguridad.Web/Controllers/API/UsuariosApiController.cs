using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers.Api
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosApiController : ControllerBase
    {
        private readonly IAdminServicio _adminServicio;
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IAuditoriaServicio _auditoriaServicio;

        public UsuariosApiController(IAdminServicio adminServicio, IUsuarioServicio usuarioServicio, IAuditoriaServicio auditoriaServicio)
        {
            _adminServicio = adminServicio;
            _usuarioServicio = usuarioServicio;
            _auditoriaServicio = auditoriaServicio;
        }

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
                mensaje = "Acceso de administrador confirmado."
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _usuarioServicio.ExisteUsuarioAsync(model.Email))
            {
                return BadRequest(new { mensaje = "Correo electronico en uso." });
            }

            var nuevoUsuario = new Usuario
            {
                UserName = model.UserName,
                Email = model.Email,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                FechaNacimiento = model.FechaNacimiento.Value,
                FechaRegistro = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var resultado = await _usuarioServicio.GuardarUsuarioAsync(nuevoUsuario, model.Password);

            if (!resultado.Succeeded)
            {
                return BadRequest(new { errores = resultado.Errors.Select(e => e.Description) });
            }

            await _usuarioServicio.AsignarRolAsync(nuevoUsuario, "Usuario");

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "sistema-api";
            var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Admin API";

            await _auditoriaServicio.RegistrarAsync(
                adminId: adminId,
                adminNombre: adminEmail,
                tipo: TipoActividad.CreacionUsuario,
                usuarioAfectadoId: nuevoUsuario.Id,
                usuarioAfectadoNombre: $"{nuevoUsuario.Nombre} {nuevoUsuario.Apellido}",
                detalle: "Rol asignado: Usuario"
            );

            return Ok(new { mensaje = $"Usuario creado exitosamente."});
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("ModificarUsuario")]
        public async Task<IActionResult> ModificarUsuario([FromBody] EditarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _usuarioServicio.ObtenerUsuarioPorIdAsync(model.Id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario que intenta modificar no existe." });
            }

            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.UserName = model.UserName;
            usuario.Email = model.Email;

            var resultado = await _usuarioServicio.ActualizarUsuarioAsync(usuario);

            if (!resultado.Succeeded)
            {
                return BadRequest(new { errores = resultado.Errors.Select(e => e.Description) });
            }

            if (!string.IsNullOrEmpty(model.Rol))
            {
                await _usuarioServicio.AsignarRolAsync(usuario, model.Rol);
            }

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "sistema-api";
            var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Admin API";

            await _auditoriaServicio.RegistrarAsync(
                adminId: adminId,
                adminNombre: adminEmail,
                tipo: TipoActividad.EdicionUsuario,
                usuarioAfectadoId: usuario.Id,
                usuarioAfectadoNombre: $"{usuario.Nombre} {usuario.Apellido}",
                detalle: null
            );

            return Ok(new { mensaje = $"Usuario {usuario.Email} actualizado correctamente." });
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(string id)
        {
            var usuario = await _usuarioServicio.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario que intenta eliminar no existe." });
            }

            await _adminServicio.EliminarUsuarioAsync(id);

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "sistema-api";
            var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Admin API";

            await _auditoriaServicio.RegistrarAsync(
                adminId: adminId,
                adminNombre: adminEmail,
                tipo: TipoActividad.EliminacionUsuario,
                usuarioAfectadoId: usuario.Id,
                usuarioAfectadoNombre: $"{usuario.Nombre} {usuario.Apellido}",
                detalle: null
            );

            return Ok(new { mensaje = $"Usuario {usuario.Email} eliminado correctamente." });
        }
    }
}