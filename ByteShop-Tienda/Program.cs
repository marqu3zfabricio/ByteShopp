using CapaDatos;
using CapaNegocio;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
;
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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/ByteShopTienda/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ByteShopTienda}/{action=Index}/{id?}");

app.Run();
