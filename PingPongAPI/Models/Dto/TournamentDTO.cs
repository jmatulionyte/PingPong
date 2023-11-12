using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongAPI.Models.Dto
{
    public class TournamentDTO
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

    }
}

