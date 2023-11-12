using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongWeb.Models.Dto
{
    public class PlayerDTO
	{
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EnrolledToTournament { get; set; }

        public int Points { get; set; }

        public string GroupName { get; set; }

        public int GroupPosition { get; set; } = 0;

        public int GroupWins { get; set; } = 0;
    }
}

