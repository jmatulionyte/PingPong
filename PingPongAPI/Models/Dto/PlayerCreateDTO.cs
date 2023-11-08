using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongAPI.Models.Dto
{
    public class PlayerCreateDTO
	{
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^(Yes||No)$", ErrorMessage = "Enrollment value can be either 'Yes' or 'No'")]
        [DisplayName("Enrolled To Tournament")]
        public string EnrolledToTournament { get; set; }

        [RegularExpression(@"^(A||B||C)$", ErrorMessage = "Group name value can be either 'A', 'B' or 'C'")]
        public string GroupName { get; set; } //it seems this is required by DB to be entered...
    }
}

