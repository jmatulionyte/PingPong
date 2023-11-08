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
using System.Numerics;

namespace PingPongAPI.Controllers;

[ApiController]
[Route("api/MatchAPI")]
public class MatchAPIController : ControllerBase
{

    protected APIResponse _response;
    private readonly IPlayerRepository _dbPlayer;
    private readonly IMatchRepository _dbMatch;
    private readonly IMapper _mapper;

    //dependency injection
    public MatchAPIController(IPlayerRepository dbPlayer, IMapper mapper, IMatchRepository dbMatch)
    {
        _dbPlayer = dbPlayer;
        _dbMatch = dbMatch;
        _mapper = mapper;
        this._response = new();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetMatches([FromQuery] string? groupName)
    {
        try
        {
            if (groupName != null)
            {
                //get all enrolled players in specific group
                List<Match> specificGroupMatches = await _dbMatch.GetAllAsync(u => u.GroupName == groupName);
                _response.Result = specificGroupMatches;
            }
            else
            {
                List<Match> matchList = await _dbMatch.GetAllAsync();
                _response.Result = _mapper.Map<List<MatchDTO>>(matchList);
            }
            
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                 = new List<string>() { ex.ToString() };
        }
        return _response;
    }


    [HttpGet("{id:int}", Name = "GetMatch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> GetMatch(int id)
    {
        try
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Id is 0");
                return BadRequest(_response);
            }
            var match = await _dbMatch.GetAsync(u => u.Id == id);
            if (match == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Match with this id is not existant");
                return NotFound(_response);
            }
            _response.Result = _mapper.Map<MatchDTO>(match);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                 = new List<string>() { ex.ToString() };
        }
        return _response;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    //[Authorize(Roles = "admin")]
    public async Task<ActionResult<APIResponse>> CreateMatches()
    {
        try
        {
            List<Match> matchList = await _dbMatch.GetAllAsync();
            if (matchList.Count() == 0)
            {
                //get Player objects with enrollment 'yes'
                List<Player> enrolledPlayerListInGroup = await _dbPlayer
                    .GetAllAsync(u =>u.EnrolledToTournament == "Yes");
                //create matches and save to DB
                await _dbMatch.CreateMatchesForGroups(enrolledPlayerListInGroup);
                //get created matches
                matchList = await _dbMatch.GetAllAsync();
                _response.Result = _mapper.Map<List<MatchDTO>>(matchList);
                _response.StatusCode = HttpStatusCode.Created;
                return (_response);
            }

            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Group matches are already created");
            return BadRequest(_response); //TODO NOW RETURNS 200, SHoULD BE 201
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages
                 = new List<string>() { ex.ToString() };
        }
        return _response;
    }

    [HttpPut("{id:int}", Name = "UpdateMatch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<APIResponse>> UpdateMatch([FromBody] MatchUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null)
            {
                _response.StatusCode = HttpStatusCode.BadGateway;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Match with this id is not existant");
                return BadRequest(_response);
            }

            Match model = _mapper.Map<Match>(updateDTO);
            await _dbMatch.UpdateAsync(model);

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }
        return _response;
    }
}

