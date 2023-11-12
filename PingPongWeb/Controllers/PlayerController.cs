using Microsoft.AspNetCore.Mvc;
using PingPongWeb.Models;
using AutoMapper;
using System.Data;
using PingPongWeb.Services.IServices;
using PingPongWeb.Models.Dto;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PingPongWeb.Controllers
{

    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly IMapper _mapper;

        //private readonly HelperPlayer _helperPlayer;

        public PlayerController(IPlayerService playerService, IMapper mapper)
        {
            _playerService = playerService;
            _mapper = mapper;
            //_helperPlayer = new HelperPlayer(_db);
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            List<PlayerDTO> list = new();

            var response = await _playerService.GetAllAsync<APIResponse>();
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<PlayerDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id)
        {
            var response = await _playerService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                //deserialize to list of dto objects
                PlayerDTO model = JsonConvert.DeserializeObject<PlayerDTO>(Convert.ToString(response.Result));
                //need to pass as VillaUpdateDTO
                return View(_mapper.Map<PlayerUpdateDTO>(model));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(PlayerUpdateDTO model)
        {
            if (ModelState.IsValid)
            {//get villa by id, desiarialize to villaDTO model and then update it
                var response = await _playerService.UpdatePlayerAsync<APIResponse>(model, HttpContext.Session.GetString(SpecialDetails.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _playerService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                //deserialize to list of dto objects
                PlayerDTO model = JsonConvert.DeserializeObject<PlayerDTO>(Convert.ToString(response.Result));
                //need to pass as VillaUpdateDTO
                return View(model);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(PlayerDTO model)
        {
            var response = await _playerService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SpecialDetails.SessionToken));
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

    }
}