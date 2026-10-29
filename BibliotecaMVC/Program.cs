using System.Globalization;
using BibliotecaMVC.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaConnection")));

var app = builder.Build();

// Datos ficticios para la demostración: completa hasta 100 socios sin reemplazar los existentes.
if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<BibliotecaContext>();
    var cantidadAgregada = await SociosSeeder.CompletarAsync(context);
    app.Logger.LogInformation("Se agregaron {Cantidad} socios de ejemplo con Bogus.", cantidadAgregada);
}

// Fechas y números en formato argentino (dd/MM/yyyy)
var culturaArgentina = new CultureInfo("es-AR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culturaArgentina),
    SupportedCultures = [culturaArgentina],
    SupportedUICultures = [culturaArgentina]
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
