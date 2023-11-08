using System;
using System.Text.RegularExpressions;
using PingPongWeb.Models.Dto;

namespace PingPongWeb.Models
{
	public class GroupMatchViewModel
	{
        public List<MatchDTO> groupA { get; set; }
        public List<MatchDTO> groupB { get; set; }
        public List<MatchDTO> groupC { get; set; }
    }
}

