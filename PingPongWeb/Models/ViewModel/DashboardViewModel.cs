using System;
using System.Text.RegularExpressions;

namespace PingPongWeb.Models
{
	public class DashboardViewModel
	{
        public GroupViewModel groupViewModel;
        public GroupMatchViewModel groupMatchViewModel;
        public PlayoffGraphViewModel playoffGraphViewModel;
        public bool tournamentStarted = false;
    }
}

