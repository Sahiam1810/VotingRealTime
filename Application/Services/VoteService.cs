using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;

namespace Application.Services
{
    public class VoteService : IVoteService
    {
        private VotingSession _session = new();

    public VotingSession GetCurrentSession() => _session;

    public void CreateSession(string question, List<string> options)
    {
        _session = new VotingSession
        {
            Question = question,
            IsActive = true,
            Options = options.ToDictionary(o => o, _ => 0)
        };
    }

    public void CastVote(string option)
    {
        if (!_session.IsActive) return;
        if (!_session.Options.ContainsKey(option)) return;

        _session.Options[option]++;
    }

    public void ResetSession()
    {
        foreach (var key in _session.Options.Keys.ToList())
            _session.Options[key] = 0;
    }
    }
}