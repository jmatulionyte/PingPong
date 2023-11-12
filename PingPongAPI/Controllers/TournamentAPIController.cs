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
    private readonly ITournamentRepository _dbTournament;

    private readonly IMapper _mapper;

    //dependency injection
    public TournamentAPIController(IMapper mapper, ITournamentRepository dbTournament)
    {
        _mapper = mapper;
        this._response = new();
        _dbTournament = dbTournament;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetTournaments()
    {
        try
        {

            //get all players
            List<Tournament> tournamentList = await _dbTournament.GetAllAsync();
            _response.Result = _mapper.Map<List<TournamentDTO>>(tournamentList);

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

    [HttpGet("{id:int}", Name = "GetTournament")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> GetTournament(int id)
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
            var tournament = await _dbTournament.GetAsync(u => u.Id == id);
            if (tournament == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Tournament with this id is not existant");
                return NotFound(_response);
            }
            _response.Result = _mapper.Map<TournamentDTO>(tournament);
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<APIResponse>> CreateTournament()
    {
        try
        {
            DateTime now = DateTime.Now;
            Tournament tournament = new()
            {
                StartDate = now,
                EndDate = new DateTime()
            };

            await _dbTournament.CreateAsync(tournament);

            _response.Result = _mapper.Map<TournamentDTO>(tournament);
            _response.StatusCode = HttpStatusCode.Created;
            //show route where can fetch resource
            return CreatedAtRoute("GetTournament", new { id = tournament.Id }, _response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }
        return _response;
    }

    [HttpPut("{id:int}", Name = "UpdateTournament")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateTournament([FromBody] TournamentUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null)
            {

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            
            //update specific player
            Tournament model = _mapper.Map<Tournament>(updateDTO);
            await _dbTournament.UpdateAsync(model);

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

