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
    private readonly IVoteService _voteService;
    private readonly IHubContext<VotingHub> _hubContext;

    public VoteController(IVoteService voteService, IHubContext<VotingHub> hubContext)
    {
        _voteService = voteService;
        _hubContext = hubContext;
    }

    [HttpGet("session")]
    public IActionResult GetSession()
    {
        var session = _voteService.GetCurrentSession();
        return Ok(session);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
    {
        _voteService.CreateSession(request.Question, request.Options);

        var session = _voteService.GetCurrentSession();

        await _hubContext.Clients.All.SendAsync("ReceiveNewSession", session);

        return Ok(session);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetSession()
    {
        _voteService.ResetSession();

        var session = _voteService.GetCurrentSession();
        await _hubContext.Clients.All.SendAsync("ReceiveVoteUpdate", session.Options);

        return Ok(session);
    }
}

public record CreateSessionRequest(string Question, List<string> Options);