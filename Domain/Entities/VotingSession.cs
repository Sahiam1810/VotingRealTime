using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VotingSession
    {
        public string? Question { get; set; }
        public Dictionary<string, int> Options { get; set; } = new();
        public bool IsActive { get; set; } = false;
    }
}