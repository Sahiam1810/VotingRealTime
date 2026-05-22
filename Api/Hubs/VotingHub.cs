using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class VotingHub : Hub // Al heredar de Hub, la clase obtiene acceso a Clients, a Context (quién está conectado), y los métodos de ciclo de vida como OnConnectedAsync, OnDisconnectedAsync, etc.
    {
        private readonly IVoteService _voteService;

        public VotingHub(IVoteService voteService)
        {
            _voteService = voteService;
        }

        // El cliente llama a este método para votar
        public async Task Vote(string option)
        {
            _voteService.CastVote(option); // // 1. Guarda el voto en memoria
            var session = _voteService.GetCurrentSession(); // 2. Lee el estado actualizado
            await Clients.All.SendAsync("ReceiveVoteUpdate", session.Options); // 3. Avisa a todos
        }

        // Se ejecuta automáticamente cuando un cliente se conecta
        public override async Task OnConnectedAsync()
        {
            var session = _voteService.GetCurrentSession();

            await Clients.Caller.SendAsync("ReceiveCurrentSession", session);  // Enviar estado actual SOLO al cliente que se acaba de conectar

            await base.OnConnectedAsync();
        }
    }
}