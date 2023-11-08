using System;
namespace PingPongAPI.Models.Dto
{
	public class RegistrationRequestDTO
	{
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}

