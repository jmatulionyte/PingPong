using System;
using PingPongWeb.Models.Dto;

namespace PingPongWeb.Models
{
    public class PlayoffGraphViewModel
    {
        public Dictionary<int, MatchDTO> PlayoffMatchesForGraph { get; set; }

        public string PlayoffWinner { get; set; }

        public string Playoff3rdPlace { get; set; }

        public bool TournamentStarted { get; set; }
    }
}

