using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;

namespace Tp_Investigacion_Ciberseguridad.Data.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<Usuario> userManager)
        {
            var usuarios = new List<(string Nombre, string Apellido, string Email, DateTime FechaRegistro)>
            {
                ("Lucas",    "Gómez",     "lucas.gomez@gmail.com",     new DateTime(2026, 4, 3)),
                ("Martina",  "Fernández", "martina.fernandez@gmail.com", new DateTime(2026, 4, 7)),
                ("Tomás",    "López",     "tomas.lopez@gmail.com",     new DateTime(2026, 4, 12)),
                ("Sofía",    "Díaz",      "sofia.diaz@gmail.com",      new DateTime(2026, 4, 18)),
                ("Mateo",    "Martínez",  "mateo.martinez@gmail.com",  new DateTime(2026, 4, 22)),
                ("Valentina","Pérez",     "valentina.perez@gmail.com", new DateTime(2026, 4, 27)),
                ("Joaquín",  "Sosa",      "joaquin.sosa@gmail.com",    new DateTime(2026, 5, 2)),
                ("Camila",   "Romero",    "camila.romero@gmail.com",   new DateTime(2026, 5, 5)),
                ("Benjamín", "Acosta",    "benjamin.acosta@gmail.com", new DateTime(2026, 5, 9)),
                ("Julieta",  "Suárez",    "julieta.suarez@gmail.com",  new DateTime(2026, 5, 13)),
                ("Agustín",  "Torres",    "agustin.torres@gmail.com",  new DateTime(2026, 5, 16)),
                ("Catalina", "Flores",    "catalina.flores@gmail.com", new DateTime(2026, 5, 20)),
                ("Nicolás",  "Benítez",   "nicolas.benitez@gmail.com", new DateTime(2026, 5, 24)),
                ("Florencia","Castro",    "florencia.castro@gmail.com",new DateTime(2026, 5, 29)),
                ("Santiago", "Ortiz",     "santiago.ortiz@gmail.com",  new DateTime(2026, 6, 1)),
                ("Emilia",   "Silva",     "emilia.silva@gmail.com",    new DateTime(2026, 6, 4)),
                ("Bautista", "Rojas",     "bautista.rojas@gmail.com",  new DateTime(2026, 6, 8)),
                ("Renata",   "Molina",    "renata.molina@gmail.com",   new DateTime(2026, 6, 12)),
                ("Ignacio",  "Vega",      "ignacio.vega@gmail.com",    new DateTime(2026, 6, 16)),
                ("Abril",    "Cabrera",   "abril.cabrera@gmail.com",   new DateTime(2026, 6, 20)),
            };

            foreach (var u in usuarios)
            {
                var existente = await userManager.FindByEmailAsync(u.Email);
                if (existente != null)
                    continue;

                var nuevoUsuario = new Usuario
                {
                    UserName = u.Email,
                    Email = u.Email,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    FechaNacimiento = new DateTime(
                        Random.Shared.Next(1980, 2006),
                        Random.Shared.Next(1, 13),
                        Random.Shared.Next(1, 28)),
                    FechaRegistro = u.FechaRegistro,
                    UltimaConexionExitosa = Random.Shared.Next(0, 2) == 1,
                    EmailConfirmed = true
                };


                var resultado = await userManager.CreateAsync(nuevoUsuario, "Usuario123!");

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(nuevoUsuario, "Usuario");
                }
            }
        }
    }
}