using System;
using PingPongWeb.Models;

namespace PingPongWeb.Services.IServices
{
    public interface IDashboard
	{
		Task<GroupViewModel> CreateGroupViewModel();
		Task<GroupMatchViewModel> CreateGroupMatchesViewModel();
        Task<PlayoffGraphViewModel>CreatePlayoffGraphViewModel();
        Task UpdateTournamentStatus();
    }
}

