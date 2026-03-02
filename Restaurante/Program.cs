using Restaurante.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel para permitir HTTP/2 sin TLS (necesario para gRPC sin certificado en dev)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, o => o.Protocols = HttpProtocols.Http2);
});

// Agregar servicios gRPC
builder.Services.AddGrpc();

// Registrar RestauranteService como Singleton para mantener el estado (la cola y el semáforo)
builder.Services.AddSingleton<RestauranteService>();

var app = builder.Build();

// Mapear el servicio gRPC y una ruta GET básica
app.MapGrpcService<RestauranteService>();
app.MapGet("/", () => "Servidor gRPC del Restaurante ejecutandose...");

Console.WriteLine("Iniciando servidor gRPC del Restaurante...");
Console.WriteLine("Presiona Ctrl+C para salir");

// Ejecutar la aplicación
app.Run();