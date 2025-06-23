using Microsoft.EntityFrameworkCore;
using Proyecto_Taqueria.Components;
using Proyecto_Taqueria.Data;
using Microsoft.AspNetCore.Diagnostics; // Necesario para IExceptionHandlerFeature
using Microsoft.AspNetCore.Http;       // Necesario para StatusCodes y WriteAsync
using Microsoft.Extensions.Logging;    // Necesario para Logging si se usa en el manejador

var builder = WebApplication.CreateBuilder(args);

// Configuración de la BD (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BarConnection"))); // Asegúrate que "BarConnection" esté en appsettings.json

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // Habilita los componentes interactivos del lado del servidor

// Configuración opcional del servicio de Antiforgery (normalmente no necesario a menos que tengas necesidades específicas)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN"; // Nombre de cabecera común para el token
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware para capturar y loguear excepciones, incluyendo AntiforgeryValidationException
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Loguea el error en la consola de Visual Studio (Ventana de Salida -> Depuración)
        Console.WriteLine($"\n--- EXCEPCIÓN DETECTADA POR MIDDLEWARE ---");
        Console.WriteLine($"Ruta: {context.Request.Path}");
        Console.WriteLine($"Tipo de Excepción: {exception?.GetType().Name}");
        Console.WriteLine($"Mensaje de Excepción: {exception?.Message}");
        Console.WriteLine($"Stack Trace: {exception?.StackTrace}");
        Console.WriteLine($"-------------------------------------------\n");

        // Si es una AntiforgeryValidationException, devuelve un mensaje específico
        if (exception is Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Error de seguridad: La solicitud no pudo ser verificada (token Antiforgery). Por favor, recargue la página si el problema persiste.");
        }
        else
        {
            // Para otros errores no manejados
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Ocurrió un error interno inesperado en el servidor.");
        }
    });
});

app.UseAntiforgery(); // Esta línea DEBE ir después de UseExceptionHandler si quieres que lo capture.

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // Asocia el componente App como la raíz de la aplicación Blazor y habilita la interactividad del servidor.

app.Run();