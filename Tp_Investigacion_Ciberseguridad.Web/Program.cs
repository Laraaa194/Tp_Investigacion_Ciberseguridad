using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Core.Servicios;
using Tp_Investigacion_Ciberseguridad.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();

builder.Services.AddDbContext<GestionUsuariosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configurar Identity apuntando a tu DbContext REAL
builder.Services.AddIdentity<Usuario, Rol>()
    .AddEntityFrameworkStores<GestionUsuariosDbContext>() // <-- Acá tiene que decir GestionUsuariosDbContext
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
