using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Data.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<Rol> roleManager, UserManager<Usuario> userManager, IConfiguration configuration)
        {
            // Crear roles si no existen
            var roles = new List<Rol>
            {
                new Rol("Admin", "Administrador del sistema con acceso total", nivelJerarquia: 1),
                new Rol("Usuario", "Usuario estándar con acceso limitado", nivelJerarquia: 2)
            };

            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol.Name))
                    await roleManager.CreateAsync(rol);
            }

            // Leer credenciales desde secrets
            var adminEmail = configuration["AdminSeed:Email"];
            var adminPassword = configuration["AdminSeed:Password"];

            // Si no están configurados los secrets, no ejecutar el seed
            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
                return;

            // Crear usuario admin si no existe
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new Usuario
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Nombre = "Admin",
                    Apellido = "Sistema",
                };

                var resultado = await userManager.CreateAsync(admin, adminPassword);

                if (resultado.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
