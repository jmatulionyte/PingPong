using Microsoft.AspNetCore.Mvc;
using PingPongWeb.Models;
using PingPongWeb.Services.IServices;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PingPongWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboard _dashboardHelper;
        private readonly ITournamentService _tournamentService;
        private readonly IGroupMatchService _groupMatchService;

        public DashboardController(IDashboard dashboardHelper, ITournamentService tournamentService, IGroupMatchService groupMatchService)
        {
            _dashboardHelper = dashboardHelper;
            _tournamentService = tournamentService;
            _groupMatchService = groupMatchService;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(bool createTournament = false)
        {
            //by default in Dashboard show group view
            DashboardViewModel dashboardViewModel = new()
            {
                groupViewModel = await _dashboardHelper.CreateGroupViewModel(),
                tournamentStarted = false
            };

            //check if tournament ongoing
            //if yes - show groups, matches, playoof graph
            //if no - show only groups
            bool tournamentOngoing = await _tournamentService.CheckIfOngoingTournament();

            
            if(createTournament && !tournamentOngoing || tournamentOngoing)
            {
                if(!tournamentOngoing) //if there are no tournament that are ongoing - start one
                {
                    var response = await _tournamentService.CreateAsync<APIResponse>();
                    if (response != null && response.IsSuccess)
                    {

                    }
                    else
                    {
                        ///??????? throw failure screen?? or would just provide empty list
                    }
                }

                //additional view when tournament is started
                dashboardViewModel.groupMatchViewModel = await _dashboardHelper.CreateGroupMatchesViewModel();
                dashboardViewModel.tournamentStarted = true;
                dashboardViewModel.playoffGraphViewModel = await _dashboardHelper.CreatePlayoffGraphViewModel();
            }

            return View(dashboardViewModel);
        }

        public async Task<IActionResult> EndTournament()
        {
            await _tournamentService.EndTournament();
            return RedirectToAction("Index", "Player");
        }

    }
}