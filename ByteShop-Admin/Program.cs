using CapaDatos;
using CapaNegocio;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using CloudinaryDotNet;
using ByteShop_Admin.Helpers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ===== Agregar servicios MVC =====
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// ===== Configurar DbContext con SQL Server =====
builder.Services.AddDbContext<MiContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadena")));

// ===== Registrar automáticamente todas las clases de datos (CD_) =====
var datosAssembly = Assembly.GetAssembly(typeof(CD_Producto)) ?? Assembly.GetExecutingAssembly();
foreach (var type in datosAssembly.GetTypes())
{
    if (type.IsClass && type.Name.StartsWith("CD_"))
    {
        builder.Services.AddScoped(type);
    }
}

// ===== Registrar automáticamente todas las clases de negocio (CN_) =====
var negocioAssembly = Assembly.GetAssembly(typeof(CN_Producto)) ?? Assembly.GetExecutingAssembly();
foreach (var type in negocioAssembly.GetTypes())
{
    if (type.IsClass && type.Name.StartsWith("CN_"))
    {
        builder.Services.AddScoped(type);
    }
}

// ===== Configuración de Cloudinary =====
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(
        config.CloudName,
        config.ApiKey,
        config.ApiSecret
    );
    return new Cloudinary(account) { Api = { Secure = true } };
});

// ===== Configurar sesiones =====
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // duración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ===== Configurar pipeline HTTP =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ===== Habilitar sesión antes de autorización y rutas =====
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
