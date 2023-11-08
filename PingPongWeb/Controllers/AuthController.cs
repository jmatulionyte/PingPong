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
using PingPongWeb.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PingPongWeb.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IPlayerService _playerService;

    public AuthController(IAuthService authService, IPlayerService playerService)
    {
        _authService = authService;
        _playerService = playerService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDTO obj = new();
        return View(obj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDTO obj)
    {
        APIResponse response = await _authService.LoginAsync<APIResponse>(obj);
        if (response != null && response.IsSuccess)
        {
            //get token
            LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

            //get claims identity from token
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);
            //tell httpcontext that login happened - set claims identity
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, model.User.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            //set claim principal with idenity
            var principal = new ClaimsPrincipal(identity);
            //http context signs in
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            //set key/ValueTask for session
            HttpContext.Session.SetString(SpecialDetails.SessionToken, model.Token);
            HttpContext.Session.SetString("Id", model.User.ID);

            return RedirectToAction("Index", "Player");
        }
        else
        {
            ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
            return View(obj);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationRequestDTO obj)
    {
        APIResponse response = await _authService.RegisterAsync<APIResponse>(obj);
        
        if (response != null && response.IsSuccess)
        {
            //after successful registration, create player object in Players tbl
            response = await _playerService.CreateAsync<APIResponse>(new PlayerCreateDTO()
            {
                FirstName = obj.FirstName,
                LastName = obj.LastName
            });

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Login");
            }
            
        }
        //handle erors like duplicate users
        return View(obj);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Session.SetString(SpecialDetails.SessionToken, "");
        return RedirectToAction("Index", "Player");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

}