using Api.Extensions;
using Api.Hubs;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios ────────────────────────────────────────────────────────────────

builder.Services.AddControllers(); // Habilita los Controllers de la API

// Capas de la arquitectura
builder.Services.AddApplicationServices();   // Registra VoteService como Singleton
builder.Services.AddInfrastructureServices(); // Vacío en este taller, listo para crecer

// CORS: permite conexiones desde otros orígenes (buena práctica, no obligatorio aquí)
builder.Services.ConfigureCors();

// SignalR: habilita la comunicación en tiempo real
builder.Services.AddSignalR();

// ─── Pipeline (orden de procesamiento de cada petición) ───────────────────────

var app = builder.Build();

// Aplica la política de CORS antes que cualquier otra cosa
app.UseCors("CorsPolicy");

// Sirve el HTML y JS que están en la carpeta wwwroot
app.UseStaticFiles();

// Enruta las peticiones HTTP a los Controllers
app.MapControllers();

// Expone el Hub en /votingHub — URL a la que se conecta el cliente JavaScript
app.MapHub<VotingHub>("/votingHub");

app.Run();