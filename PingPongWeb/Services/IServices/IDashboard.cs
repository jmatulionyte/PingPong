using System;
using PingPongWeb.Models;

namespace PingPongWeb.Services.IServices
{
    public interface IDashboard
	{
		Task<GroupViewModel> CreateGroupViewModel();
		//duplicate almost, ask egida if can decrease fnctions
		Task<GroupMatchViewModel> CreateGroupMatchesViewModel();

    }
}

