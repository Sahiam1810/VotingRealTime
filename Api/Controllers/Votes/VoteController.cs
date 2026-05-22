using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;

namespace Api.Controllers.Votes;
[ApiController]
[Route("api/[controller]")]

public class VoteController : ControllerBase
{
    private readonly IVoteService _voteService; // servicio de votación
    private readonly IHubContext<VotingHub> _hubContext; // puente entre controller y Hub

    public VoteController(IVoteService voteService, IHubContext<VotingHub> hubContext) 
    {
        _voteService = voteService;
        _hubContext = hubContext;
    }

    [HttpGet("session")] // endpoint para obtener el estado actual de la votación
    public IActionResult GetSession() // get simple que devuelve el estado actual de la votación
    {
        var session = _voteService.GetCurrentSession();
        return Ok(session);
    }

    [HttpPost("create")] // endpoint para crear una nueva sesión de votación
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request) // post que recibe el body con la pregunta y opciones y crea una nueva sesión de votación
    {
        _voteService.CreateSession(request.Question, request.Options);

        var session = _voteService.GetCurrentSession();

        await _hubContext.Clients.All.SendAsync("ReceiveNewSession", session); // envia el estado actual a todos los clientes

        return Ok(session);
    }

    [HttpPost("reset")] // endpoint para resetear los votos de la sesión actual
    public async Task<IActionResult> ResetSession()
    {
        _voteService.ResetSession();

        var session = _voteService.GetCurrentSession();
        await _hubContext.Clients.All.SendAsync("ReceiveVoteUpdate", session.Options); // Enviar los votos actuales a todos los clientes

        return Ok(session);
    }
}

public record CreateSessionRequest(string Question, List<string> Options); 