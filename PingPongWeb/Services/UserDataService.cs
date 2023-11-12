using System;
using Newtonsoft.Json;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;
using PingPongWeb.Services.IServices;
using Utility;

namespace PingPongWeb.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly IPlayerService _playerService;
        private readonly IGroupMatchService _matchService;

        public UserDataService(IPlayerService playerService, IGroupMatchService matchService)
        {
            _playerService = playerService;
            _matchService = matchService;
        }

        public async Task<List<MatchDTO>> GetUserMatches(string fullName)
        {
            List<MatchDTO> matchesForUser = await _matchService.GetGroupMatchesForUser(fullName);
            if(matchesForUser.Count() != 0)
            {
                return matchesForUser;
            }
            return new List<MatchDTO>();
        }
    }
}

