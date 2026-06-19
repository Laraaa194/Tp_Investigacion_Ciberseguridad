using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tp_Investigacion_Ciberseguridad.Core.Entidades;
using Tp_Investigacion_Ciberseguridad.Core.Interfaces;
using Tp_Investigacion_Ciberseguridad.Data;
using Tp_Investigacion_Ciberseguridad.Data.Identity;
using Tp_Investigacion_Ciberseguridad.Data.Seeders;
using Tp_Investigacion_Ciberseguridad.Data.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminServicio, AdminServicio>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();

builder.Services.AddDbContext<GestionUsuariosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Usuario, Rol>(options =>
{
    // Configuración de lockout (fuerza bruta)
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.AllowedForNewUsers = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddErrorDescriber<IdentityErrorDescriberEsp>()
.AddEntityFrameworkStores<GestionUsuariosDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection =
            builder.Configuration.GetSection("Authentication:Google");

        // Lee las credenciales del appsettings
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//Configuracion base cookies
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Solo enviar cookies por HTTPS, nunca por HTTP
    options.Secure = CookieSecurePolicy.Always;

    // Impedir que JavaScript acceda a las cookies (protección contra XSS)
    options.HttpOnly = HttpOnlyPolicy.Always;

    // Controlar desde qué sitios se pueden enviar las cookies (protección contra CSRF)
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// Configuración de la cookie de Identity (específica)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/InicioSesion";
    options.AccessDeniedPath = "/Account/AccesoDenegado";
});

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
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Rol>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await RoleSeeder.SeedAsync(roleManager, userManager, configuration);
}

app.Run();
