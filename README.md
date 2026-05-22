# VotingRealTime

Sistema de votaciones en tiempo real construido con **ASP.NET Core 10** y **SignalR**, usando arquitectura por capas y frontend en HTML + JavaScript puro.

---

## Descripción

VotingRealTime es un taller guiado diseñado para aprender a usar **SignalR** con buenas prácticas de arquitectura. Permite crear sesiones de votación en las que múltiples usuarios conectados ven los resultados actualizarse en tiempo real sin necesidad de recargar la página.

---

## Funcionalidades

- Crear una sesión de votación con pregunta y opciones personalizadas
- Votar en tiempo real haciendo clic en una opción
- Ver los resultados actualizarse instantáneamente en todos los navegadores conectados
- Resetear los votos de la sesión activa
- Estado sincronizado: los usuarios que se conectan tarde ven el estado actual de la votación

---

## Tecnologías utilizadas

| Tecnología | Versión | Uso |
|---|---|---|
| .NET | 10 | Framework principal |
| ASP.NET Core | 10 | API REST y servidor |
| SignalR | Incluido en ASP.NET Core | Comunicación en tiempo real |
| C# | 13 | Lenguaje de programación |
| HTML + JavaScript | - | Frontend del cliente |
| WebSockets | - | Transporte de SignalR |

---

## Arquitectura

El proyecto sigue una **arquitectura por capas** inspirada en Clean Architecture:

```
VotingRealTime/
├── Api/                        → Punto de entrada (Controllers, Hub, Frontend)
│   ├── Controllers/
│   │   └── Votes/
│   │       └── VoteController.cs
│   ├── Extensions/
│   │   └── CorsServiceExtensions.cs
│   ├── Hubs/
│   │   └── VotingHub.cs        ← Hub de SignalR (corazón del proyecto)
│   ├── wwwroot/
│   │   ├── index.html          ← Frontend
│   │   └── js/
│   │       └── voting.js       ← Lógica cliente SignalR
│   └── Program.cs
├── Application/                → Lógica de negocio
│   ├── Abstractions/
│   │   └── IVoteService.cs
│   ├── Services/
│   │   └── VoteService.cs
│   ├── ApplicationAssemblyReference.cs
│   └── DependencyInjection.cs
├── Domain/                     → Entidades del negocio
│   └── Entities/
│       └── VotingSession.cs
├── Infrastructure/             → Capa de infraestructura
│   └── DependencyInjection.cs
└── VotingRealTime.sln
```

### ¿Qué hace cada capa?

**Domain** es el núcleo del sistema. Contiene las entidades que representan los conceptos del negocio. No depende de ninguna otra capa.

**Application** contiene la lógica de negocio. Define las interfaces (abstracciones) y sus implementaciones. En este proyecto el `VoteService` mantiene el estado de las votaciones en memoria.

**Infrastructure** es la capa que se comunicaría con recursos externos como base de datos o APIs. En este taller está vacía intencionalmente porque todo va en memoria, pero existe por buena práctica arquitectural.

**Api** es el punto de entrada. Contiene los Controllers, el Hub de SignalR y el frontend estático servido desde `wwwroot`.

---

## ¿Qué es SignalR?

SignalR es una librería de ASP.NET que permite **comunicación en tiempo real** entre el servidor y los clientes conectados.

La diferencia con una API REST normal:

- **REST tradicional:** El cliente pregunta → el servidor responde. Para ver datos nuevos hay que volver a preguntar.
- **SignalR:** El servidor puede enviar datos a los clientes al instante, sin que ellos lo pidan.

Por debajo, SignalR usa **WebSockets** como transporte preferido (conexión bidireccional persistente). Si el navegador no los soporta, cae automáticamente a alternativas.

### El Hub

El Hub es el concepto central de SignalR. Es una clase en el servidor que actúa como punto de encuentro entre todos los clientes conectados.

```csharp
public class VotingHub : Hub
{
    // El cliente llama a este método para votar
    public async Task Vote(string option)
    {
        _voteService.CastVote(option);
        var session = _voteService.GetCurrentSession();

        // Enviar resultados a TODOS los clientes conectados
        await Clients.All.SendAsync("ReceiveVoteUpdate", session.Options);
    }

    // Se ejecuta automáticamente cuando alguien se conecta
    public override async Task OnConnectedAsync()
    {
        var session = _voteService.GetCurrentSession();

        // Enviar estado actual solo al cliente recién conectado
        await Clients.Caller.SendAsync("ReceiveCurrentSession", session);

        await base.OnConnectedAsync();
    }
}
```

### Flujo de comunicación

```
Frontend (navegador)
        │
        ├── fetch("/api/vote/create")  →  VoteController  (HTTP normal)
        │
        └── connection.invoke("Vote") →  VotingHub        (WebSocket)
                    ↑
        connection.on("ReceiveVoteUpdate")  ←  Clients.All.SendAsync(...)
```

---

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/)
- Extensiones recomendadas para VS Code:
  - C# Dev Kit
  - C# Extensions
  - NuGet Gallery

---

## Cómo correr el proyecto

### 1. Clonar el repositorio

```bash
git clone https://github.com/TU_USUARIO/VotingRealTime.git
cd VotingRealTime
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Compilar el proyecto

```bash
dotnet build
```

### 4. Correr el servidor

```bash
cd Api
dotnet run
```

### 5. Abrir en el navegador

Busca en la consola la URL que aparece, por ejemplo:

```
Now listening on: http://localhost:5069
```

Abre `http://localhost:5069/index.html` en el navegador.

### 6. Probar en tiempo real

Abre la misma URL en **dos o más pestañas**. Crea una sesión de votación desde el panel de administrador y vota en una pestaña. Verás cómo los resultados se actualizan en todas las pestañas al instante.

---

## Compartir en red local (mismo WiFi)

Para que otras personas en la misma red puedan conectarse, busca tu IP local:

```bash
# Windows
ipconfig

# Linux / Mac
ifconfig
```

Busca la dirección **IPv4** y comparte la URL:

```
http://TU_IP:5069/index.html
```

---

## Compartir por internet con ngrok

Para que cualquier persona con internet pueda conectarse, usa [ngrok](https://ngrok.com):

### Instalar ngrok

```bash
# Windows
winget install ngrok.ngrok

# Linux
sudo apt install ngrok
```

### Configurar token

```bash
ngrok config add-authtoken TU_TOKEN
```

Obtén tu token gratis registrándote en [ngrok.com](https://ngrok.com).

### Exponer el servidor

Con el proyecto corriendo en otra terminal:

```bash
ngrok http http://localhost:5069
```

ngrok generará una URL pública como `https://abc123.ngrok-free.app`. Comparte esa URL y cualquier persona puede conectarse desde cualquier dispositivo.

---

## Paquetes utilizados

### Application
| Paquete | Versión | Descripción |
|---|---|---|
| MediatR | 14.0.0 | Patrón mediador |
| FluentValidation | última | Validaciones |
| FluentValidation.DependencyInjectionExtensions | última | Inyección de validaciones |

### Api
| Paquete | Versión | Descripción |
|---|---|---|
| MediatR | 14.0.0 | Patrón mediador |

> **Nota:** SignalR ya viene incluido en ASP.NET Core. No requiere instalar ningún paquete adicional en el servidor.

---

## Endpoints disponibles

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/vote/session` | Obtener el estado actual de la votación |
| `POST` | `/api/vote/create` | Crear una nueva sesión de votación |
| `POST` | `/api/vote/reset` | Resetear los votos de la sesión actual |
| `WS` | `/votingHub` | Endpoint WebSocket del Hub de SignalR |

### Ejemplo de body para crear sesión

```json
{
  "question": "¿Cuál es tu lenguaje favorito?",
  "options": ["C#", "Python", "JavaScript"]
}
```

---

## Autora

Sahiam Valentina Esteban