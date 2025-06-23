using Microsoft.EntityFrameworkCore;
using Proyecto_Taqueria.Components;
using Proyecto_Taqueria.Data;
using Microsoft.AspNetCore.Diagnostics; // Necesario para IExceptionHandlerFeature
using Microsoft.AspNetCore.Http;       // Necesario para StatusCodes y WriteAsync
using Microsoft.Extensions.Logging;    // Necesario para Logging si se usa en el manejador

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de la BD (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BarConnection"))); // Aseg�rate que "BarConnection" est� en appsettings.json

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // Habilita los componentes interactivos del lado del servidor

// Configuraci�n opcional del servicio de Antiforgery (normalmente no necesario a menos que tengas necesidades espec�ficas)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN"; // Nombre de cabecera com�n para el token
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

        // Loguea el error en la consola de Visual Studio (Ventana de Salida -> Depuraci�n)
        Console.WriteLine($"\n--- EXCEPCI�N DETECTADA POR MIDDLEWARE ---");
        Console.WriteLine($"Ruta: {context.Request.Path}");
        Console.WriteLine($"Tipo de Excepci�n: {exception?.GetType().Name}");
        Console.WriteLine($"Mensaje de Excepci�n: {exception?.Message}");
        Console.WriteLine($"Stack Trace: {exception?.StackTrace}");
        Console.WriteLine($"-------------------------------------------\n");

        // Si es una AntiforgeryValidationException, devuelve un mensaje espec�fico
        if (exception is Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Error de seguridad: La solicitud no pudo ser verificada (token Antiforgery). Por favor, recargue la p�gina si el problema persiste.");
        }
        else
        {
            // Para otros errores no manejados
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Ocurri� un error interno inesperado en el servidor.");
        }
    });
});

app.UseAntiforgery(); // Esta l�nea DEBE ir despu�s de UseExceptionHandler si quieres que lo capture.

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // Asocia el componente App como la ra�z de la aplicaci�n Blazor y habilita la interactividad del servidor.

app.Run();