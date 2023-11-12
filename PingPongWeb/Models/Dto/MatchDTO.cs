using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongWeb.Models.Dto
{
    public class MatchDTO
	{
        [Required]
        public int Id { get; set; }

        [Required]
        public string Player1 { get; set; }

        [Required]
        public string Player2 { get; set; }

        public int ResultPlayer1 { get; set; } = 0;

        public int ResultPlayer2 { get; set; } = 0;

        public string GroupName { get; set; }

        public string MatchType { get; set; }

        public int PlayoffMatchNr { get; set; } = 0;

        public string Winner { get; set; }
    }
}

