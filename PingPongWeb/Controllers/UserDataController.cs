using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using Utility;
using PingPongWeb.Models;
using PingPongWeb.Services.IServices;
using PingPongWeb.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Collections.Generic;
using PingPongWeb.Services;
using AutoMapper;

namespace PingPongWeb.Controllers;

//User Info - name, surname, email, points, ranking - group plays, playoff plays
public class UserDataController : Controller
{
    private readonly IAuthService _authService;
    private readonly IPlayerService _playerService;
    private readonly IGroupMatchService _matchService;
    private readonly IUserDataService _userDataService;
    private readonly IMapper _mapper;

    public UserDataController(IAuthService authService, IUserDataService userDataService, IGroupMatchService matchService,
        IMapper mapper, IPlayerService playerService)
    {
        _authService = authService;
        _userDataService = userDataService;
        _matchService = matchService;
        _mapper = mapper;
        _playerService = playerService;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Index()
    {
        string fullName = "";
        string userId = HttpContext.Session.GetString("Id");

        //string role = HttpContext.Session.GetString("Role");
        var response = await _authService.GetUserData<APIResponse>(userId);
        if (response != null && response.IsSuccess)
        {
            fullName = (string)response.Result;

        }
        List<MatchDTO> userMatches = await _userDataService.GetUserMatches(fullName);
        return View(userMatches);
    }
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id)
    {
        var response = await _matchService.GetAsync<APIResponse>(id);
        if (response != null && response.IsSuccess)
        {
            //deserialize to list of dto objects
            MatchDTO model = JsonConvert.DeserializeObject<MatchDTO>(Convert.ToString(response.Result));
            //need to pass as VillaUpdateDTO
            return View(_mapper.Map<MatchUpdateDTO>(model));
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(MatchUpdateDTO model)
    {
        if (ModelState.IsValid)
        {
            if (model.ResultPlayer1 == model.ResultPlayer2) //DOES NOT WORK FOR SOME REASON
            {
                ModelState.AddModelError("CustomError", "Player 1 Result cannot match Player 2 Result");
                return View(model);
            }

            var response = await _matchService.UpdateAsync<APIResponse>(model);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
        }
        return View(model);
    }

}