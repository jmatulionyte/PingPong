using System;
using Newtonsoft.Json;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;
using PingPongWeb.Services.IServices;

namespace PingPongWeb.Services
{
    public class Dashboard : IDashboard
    {
        private readonly IPlayerService _playerService;
        private readonly IGroupMatchService _matchService;
        private readonly IPlayoffGraphService _playoffGraphService;
        private readonly ITournamentService _tournamentService;


        public Dashboard(IPlayerService playerService, IGroupMatchService matchService, IPlayoffGraphService playoffGraphService, ITournamentService tournamentService)
        {
            _playerService = playerService;
            _matchService = matchService;
            _playoffGraphService = playoffGraphService;
            _tournamentService = tournamentService;
        }

        public async Task<GroupViewModel> CreateGroupViewModel()
        {
            List<string> groupA = await _playerService.GetSpecificGroupPlayers("A");
            List<string> groupB = await _playerService.GetSpecificGroupPlayers("B");
            List<string> groupC = await _playerService.GetSpecificGroupPlayers("C");
            return new GroupViewModel()
            {
                GroupA = groupA,
                GroupB = groupB,
                GroupC = groupC
            };
        }

        public async Task<GroupMatchViewModel> CreateGroupMatchesViewModel()
        {
            List<MatchDTO> groupA = await _matchService.GetSpecificGroupMatches("A");
            List<MatchDTO> groupB = await _matchService.GetSpecificGroupMatches("B");
            List<MatchDTO> groupC = await _matchService.GetSpecificGroupMatches("C");
            return new GroupMatchViewModel ()
            {
                GroupA = groupA,
                GroupB = groupB,
                GroupC = groupC
            };
        }

        //public async Task<GroupMatchViewModel> CreatePlayoffMatchesViewModel()
        //{
        //    List<MatchDTO> groupA = await _matchService.GetSpecificGroupMatches("A");
        //    List<MatchDTO> groupB = await _matchService.GetSpecificGroupMatches("B");
        //    List<MatchDTO> groupC = await _matchService.GetSpecificGroupMatches("C");

        //    //add all to groupView model
        //    GroupMatchViewModel groupMatchViewModel = new()
        //    {
        //        GroupA = groupA,
        //        GroupB = groupB,
        //        GroupC = groupC
        //    };
        //    return groupMatchViewModel;
        //}

        public Task UpdateTournamentStatus()
        {
            throw new NotImplementedException();
        }

        public async Task<PlayoffGraphViewModel> CreatePlayoffGraphViewModel()
        {
            Dictionary<int, MatchDTO> playoffMatchesForGraph = await _playoffGraphService.ConvertPlayoffMatchesDataForGraph();

            //get tournament status
            bool tournamentStarted = await _tournamentService.CheckIfOngoingTournament();

            //get playoff winer
            string playoffWinner = await _playoffGraphService.GetPlayoffsWinner();

            //get playoffs 3rd place winner
            string playoff3rdPlace = await _playoffGraphService.GetPlayoffs3rdPlace();

            return new PlayoffGraphViewModel()
            {
                //assign data to playoffsGraphData interface
                PlayoffMatchesForGraph = playoffMatchesForGraph,
                TournamentStarted = tournamentStarted,
                PlayoffWinner = playoffWinner,
                Playoff3rdPlace = playoff3rdPlace
            };
        }
    }
}

