// 1. Crear la conexión al Hub, apunta a la url creada en el archivo program.cs
const connection = new signalR.HubConnectionBuilder() 
    .withUrl("/votingHub") // URL del Hub
    .withAutomaticReconnect() // conexión automática
    .build(); // construye la conexión

// ─── Escuchar eventos del servidor ───────────────────────────────────────────

// El servidor nos envía el estado actual al conectarnos (OnConnectedAsync)
connection.on("ReceiveCurrentSession", function (session) { 
    renderSession(session); 
});

// El servidor nos avisa cuando alguien crea una nueva sesión
connection.on("ReceiveNewSession", function (session) {
    renderSession(session);
});

// El servidor nos envía los votos actualizados cada vez que alguien vota
connection.on("ReceiveVoteUpdate", function (options) {
    updateVoteCounts(options);
});

// ─── Iniciar la conexión ──────────────────────────────────────────────────────

connection.start() // inicia la conexión
    .then(function () { 
        document.getElementById("status").textContent = " Conectado en tiempo real"; 
        document.getElementById("status").className = "connected";
        console.log("Conectado al Hub de SignalR");
    })
    .catch(function (err) {
        document.getElementById("status").textContent = " Error de conexión";
        document.getElementById("status").className = "disconnected";
        console.error("Error al conectar:", err.toString());
    });

// ─── Funciones del cliente ────────────────────────────────────────────────────

// Votar por una opción - llama al método Vote del Hub directamente
function castVote(option) {
    connection.invoke("Vote", option) // Esto llega al método Vote() de VotingHub
        .catch(function (err) {
            console.error("Error al votar:", err.toString());
        });
}

// Crear nueva sesión - llama al API REST
async function createSession() {
    const question = document.getElementById("input-question").value.trim();
    const optionsRaw = document.getElementById("input-options").value.trim();

    if (!question || !optionsRaw) { // Si no se completa la pregunta o las opciones, muestra un mensaje de error
        alert("Completa la pregunta y las opciones");
        return;
    }

    const options = optionsRaw.split(",").map(o => o.trim()).filter(o => o);

    try {
        await fetch("/api/vote/create", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ question, options })
        });
        document.getElementById("input-question").value = "";
        document.getElementById("input-options").value = "";
    } catch (err) {
        console.error("Error al crear sesión:", err);
    }
}

// Resetear votos - llama al API REST
async function resetVotes() {
    try {
        await fetch("/api/vote/reset", { method: "POST" });
    } catch (err) {
        console.error("Error al resetear:", err);
    }
}

// ─── Renderizado ──────────────────────────────────────────────────────────────

function renderSession(session) {
    document.getElementById("question").textContent = session.question || "Sin pregunta";

    const container = document.getElementById("options-container");
    container.innerHTML = "";

    const total = Object.values(session.options).reduce((a, b) => a + b, 0);

    for (const [option, count] of Object.entries(session.options)) {
        const pct = total > 0 ? Math.round((count / total) * 100) : 0;

        const div = document.createElement("div");
        div.className = "vote-option";
        div.id = `option-${option}`;
        div.innerHTML = `
            <div class="option-label">
                <span class="option-name">${option}</span>
                <span class="option-count" id="count-${option}">${count} votos (${pct}%)</span>
            </div>
            <div class="progress-bar-bg" onclick="castVote('${option}')" title="Clic para votar">
                <div class="progress-bar" id="bar-${option}" style="width: ${pct}%"></div>
            </div>
        `;
        container.appendChild(div);
    }

    updateTotalVotes(total);
}

function updateVoteCounts(options) {
    const total = Object.values(options).reduce((a, b) => a + b, 0);

    for (const [option, count] of Object.entries(options)) {
        const pct = total > 0 ? Math.round((count / total) * 100) : 0;

        const countEl = document.getElementById(`count-${option}`);
        const barEl = document.getElementById(`bar-${option}`);

        if (countEl) countEl.textContent = `${count} votos (${pct}%)`;
        if (barEl) barEl.style.width = `${pct}%`;
    }

    updateTotalVotes(total);
}

function updateTotalVotes(total) {
    document.getElementById("total-votes").textContent =
        total > 0 ? `Total de votos: ${total}` : "";
}