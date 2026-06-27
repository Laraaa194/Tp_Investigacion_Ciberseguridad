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
        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAuditoriaServicio _auditoriaServicio;

        public AuthApiController(UserManager<Usuario> userManager, IConfiguration configuration, IAuditoriaServicio auditoriaServicio)
        {
            _userManager = userManager;
            _configuration = configuration;
            _auditoriaServicio = auditoriaServicio;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _userManager.FindByEmailAsync(model.Email);

            if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, model.Password))
            {
                if (usuario != null)
                {
                    await _auditoriaServicio.RegistrarAsync(
                        adminId: "sistema-api",
                        adminNombre: "Sistema API",
                        tipo: TipoActividad.LoginFallido,
                        usuarioAfectadoId: usuario.Id,
                        usuarioAfectadoNombre: usuario.Email ?? "Desconocido",
                        detalle: "Intento de login fallido vía API (JWT)"
                    );
                }

                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            // 5. Claims: datos que van adentro del token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID token
            };

            // Agregamos todos los roles que tenga el usuario a los claims
            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            // 6. Preparamos la Clave Secreta 
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 7. Token con su fecha de vencimiento 
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

            // 8. Devolvemos el JSON de éxito con el token
            return Ok(new
            {
                mensaje = "Login exitoso",
                token = tokenString,
                expiracion = expiracion
            });
        }
    }
}