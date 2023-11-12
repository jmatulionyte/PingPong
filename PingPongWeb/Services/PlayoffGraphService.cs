using System;
using Newtonsoft.Json;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;
using PingPongWeb.Services.IServices;

namespace PingPongWeb.Services
{
	public class PlayoffGraphService : IPlayoffGraphService
	{
        private readonly IGroupMatchService _matchService;

        public PlayoffGraphService(IGroupMatchService matchService)
        {
            _matchService = matchService;
        }

        public async Task<Dictionary<int, MatchDTO>> ConvertPlayoffMatchesDataForGraph()
        {
            Dictionary<int, MatchDTO> playoffMatchesByMatchNr = new();
            //get playoff matches from DB

            List<MatchDTO> playoffMatches = await GetMatchesForPlayoffs();
                
            foreach (var match in playoffMatches)
            {
                playoffMatchesByMatchNr[match.PlayoffMatchNr] = match;
            }
            
            return playoffMatchesByMatchNr;
        }

        private async Task<List<MatchDTO>> GetMatchesForPlayoffs()
        {
            List<MatchDTO> playoffMatches = new();
            var response = await _matchService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                playoffMatches = JsonConvert.DeserializeObject<List<MatchDTO>>(Convert.ToString(response.Result));

            }
            return playoffMatches.Where(x => x.MatchType == "Playoff").Select(x => x).ToList();
        }

        /// <summary>
        /// Get winner of Playoffs from Matches DB (Could be null if playoff not finished)  
        /// </summary>
        public async Task<string> GetPlayoffsWinner()
        {
            List<MatchDTO> allPlayoffs = await GetMatchesForPlayoffs();
            return allPlayoffs.Where(x => x.PlayoffMatchNr == 19).Select(x => x.Winner).SingleOrDefault();
        }

        /// <summary>
        /// Get 3rd place of Playoffs from Matches DB (Could be null if playoff not finished)  
        /// </summary>
        public async Task<string> GetPlayoffs3rdPlace()
        {
            List<MatchDTO> allPlayoffs = await GetMatchesForPlayoffs();
            return allPlayoffs.Where(x => x.PlayoffMatchNr == 20).Select(x => x.Winner).SingleOrDefault();
        }

        //public void FinalizeTournamentData()
        //{
        //    //count playoff wins and according to them set points for players table (ratings)

        //    //delete group games
        //    var groupResultsObj = GetGroupResults();
        //    _db.GroupResults.RemoveRange(groupResultsObj);

        //    //delete group DB
        //    var groupObj = _db.Matches;
        //    _db.Matches.RemoveRange(groupObj);

        //    //delete group DB
        //    var tournamentObj = _db.Tournaments;
        //    _db.Tournaments.RemoveRange(tournamentObj);

        //    _db.SaveChanges();

        //}
    }
}


