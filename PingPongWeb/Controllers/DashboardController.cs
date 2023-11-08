using Microsoft.AspNetCore.Mvc;
using PingPongWeb.Models;
using PingPongWeb.Services.IServices;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PingPongWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboard _dashboardHelper;

        //private readonly HelperPlayer _helperPlayer;

        public DashboardController(IDashboard dashboardHelper)
        {
            _dashboardHelper = dashboardHelper;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            GroupViewModel groupViewModel = await _dashboardHelper.CreateGroupViewModel();
            GroupMatchViewModel groupMatchViewModel = await _dashboardHelper.CreateGroupMatchesViewModel();

            DashboardViewModel dashboardViewModel = new()
            {
                groupViewModel = groupViewModel,
                groupMatchViewModel = groupMatchViewModel
            };

            return View(dashboardViewModel);
        }

    }
}