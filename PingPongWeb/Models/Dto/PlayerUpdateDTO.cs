using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongWeb.Models.Dto
{
    public class PlayerUpdateDTO
	{
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^(Yes||No)$", ErrorMessage = "Enrollment value can be either 'Yes' or 'No'")]
        public string EnrolledToTournament { get; set; }

        public string GroupName { get; set; }
    }
}

