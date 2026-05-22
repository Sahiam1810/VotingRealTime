using Api.Extensions;
using Api.Hubs;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // registrar servicios de controladores

// Nuestras capas
builder.Services.AddApplicationServices(); // Registra VoteService como Singleton
builder.Services.AddInfrastructureServices(); // Vacio en este taller

// CORS
builder.Services.ConfigureCors(); // Configura CORS

// SignalR
builder.Services.AddSignalR(); // Registra SignalR

var app = builder.Build(); // construye la aplicación

app.UseCors("CorsPolicy"); // aplica cors

// Servir archivos estáticos desde wwwroot (el HTML y JS)
app.UseStaticFiles();

app.MapControllers();

// Registrar el Hub en la ruta /votingHub
app.MapHub<VotingHub>("/votingHub");

app.Run();