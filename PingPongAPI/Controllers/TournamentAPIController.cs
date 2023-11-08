using Azure;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PingPongAPI.Models;
using PingPongAPI.Repository.IRepository;
using AutoMapper;
using PingPongAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace PingPongAPI.Controllers;

[ApiController]
[Route("api/TournamentAPI")]
public class TournamentAPIController : ControllerBase
{

    protected APIResponse _response;
    private readonly IPlayerRepository _dbPlayer;
    private readonly IMapper _mapper;

    //dependency injection
    public TournamentAPIController(IPlayerRepository dbPlayer, IMapper mapper)
    {
        _dbPlayer = dbPlayer;
        _mapper = mapper;
        this._response = new();
    }

    //get tournament data, post data, update data, deleyte?
    [HttpPost] 
    [ProducesResponseType(StatusCodes.Status200OK)]
    //UPDATE
    public async Task<ActionResult<APIResponse>> xxx()
    {
        try
        {

            return Ok();
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                 = new List<string>() { ex.ToString() };
        }
        return _response;
    }


}

