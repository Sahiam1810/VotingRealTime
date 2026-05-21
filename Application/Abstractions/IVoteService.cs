using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions
{
    public interface IVoteService
    {
        VotingSession GetCurrentSession();
        void CastVote(string option);
        void CreateSession(string question, List<string> options);
        void ResetSession();
    }
}
