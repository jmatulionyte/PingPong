using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongAPI.Models.Dto
{
    public class PlayerUpdateGroupDTO
	{
        [Required]
        public int Id { get; set; }

        public int Points { get; set; }

        public string GroupName { get; set; }

        public int GroupPosition { get; set; } = 0;

        public int GroupWins { get; set; } = 0;

    }
}

