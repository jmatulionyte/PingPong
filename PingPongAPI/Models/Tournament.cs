using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongAPI.Models
{
    public class Tournament
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

    } 
}

