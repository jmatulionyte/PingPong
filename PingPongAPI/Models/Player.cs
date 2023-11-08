using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongAPI.Models
{
    public class Player
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string EnrolledToTournament { get; set; } = "No";

        public int Points { get; set; } = 0;

        public string GroupName { get; set; }

        public int GroupPosition { get; set; } = 0;

        public int GroupWins { get; set; } = 0;
    }
}

