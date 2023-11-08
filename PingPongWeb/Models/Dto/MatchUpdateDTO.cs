using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongWeb.Models.Dto
{
    public class MatchUpdateDTO
	{
        public int Id { get; set; }

        public string Player1 { get; set; }

        public string Player2 { get; set; }

        [Required]
        public int ResultPlayer1 { get; set; } = 0;

        [Required]
        public int ResultPlayer2 { get; set; } = 0;

        public string GroupName { get; set; }

        public string MatchType { get; set; }

    }
}

