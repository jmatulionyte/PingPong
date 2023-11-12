using System;
using System.Text.RegularExpressions;
using PingPongWeb.Models.Dto;

namespace PingPongWeb.Models
{
	public class GroupMatchViewModel
	{
        public List<MatchDTO> GroupA { get; set; }
        public List<MatchDTO> GroupB { get; set; }
        public List<MatchDTO> GroupC { get; set; }
    }
}

