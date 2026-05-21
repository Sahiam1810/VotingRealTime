using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class VotingHub : Hub
    {
        private readonly IVoteService _voteService;

        public VotingHub(IVoteService voteService)
        {
            _voteService = voteService;
        }

        // El cliente llama a este método para votar
        public async Task Vote(string option)
        {
            _voteService.CastVote(option);

            var session = _voteService.GetCurrentSession();

            // Enviar resultados actualizados a TODOS los clientes conectados
            await Clients.All.SendAsync("ReceiveVoteUpdate", session.Options);
        }

        // Se ejecuta automáticamente cuando un cliente se conecta
        public override async Task OnConnectedAsync()
        {
            var session = _voteService.GetCurrentSession();

            // Enviar estado actual SOLO al cliente que se acaba de conectar
            await Clients.Caller.SendAsync("ReceiveCurrentSession", session);

            await base.OnConnectedAsync();
        }
    }
}