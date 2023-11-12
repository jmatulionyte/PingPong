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
        [Range(0, 4, ErrorMessage = "Result can be in range 0-4")]
        public int ResultPlayer1 { get; set; } = 0;

        [Required]
        [Range(0, 4, ErrorMessage = "Result can be in range 0-4")]
        public int ResultPlayer2 { get; set; } = 0;

        public string GroupName { get; set; }

        public string MatchType { get; set; }

        public string Winner { get; set; }
    }
}

