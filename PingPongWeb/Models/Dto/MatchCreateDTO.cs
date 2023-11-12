using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongWeb.Models.Dto
{
    public class MatchCreateDTO
	{
        public MatchCreateDTO(string player1, string player2, int playoffMatchNr, string matchType)
        {
            Player1 = player1;
            Player2 = player2;
            PlayoffMatchNr = playoffMatchNr;
            MatchType = matchType;
        }
        [Required]
        public string Player1 { get; set; }

        [Required]
        public string Player2 { get; set; }

        [Required]
        public string MatchType { get; set; }

        [Required]
        public int PlayoffMatchNr { get; set; } = 0;

        public string Winner { get; set; }
    }
}

