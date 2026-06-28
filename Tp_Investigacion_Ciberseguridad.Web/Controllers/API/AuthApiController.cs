using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels;

namespace Tp_Investigacion_Ciberseguridad.Web.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IAuditoriaServicio _auditoriaServicio;

        public AuthApiController( IConfiguration configuration, IUsuarioServicio usuarioServicio, IAuditoriaServicio auditoriaServicio)
        {
            _configuration = configuration;
            _usuarioServicio = usuarioServicio;
            _auditoriaServicio = auditoriaServicio;
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

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
                    new Claim(JwtRegisteredClaimNames.Email, usuario.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var secretKey = _configuration["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddHours(2);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiracion,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            return Ok(new
            {
                mensaje = "Login exitoso",
                token = tokenString,
                expiracion = expiracion
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

            return Ok(new { mensaje = "Usuario registrado con exito" });
        }
    }
}