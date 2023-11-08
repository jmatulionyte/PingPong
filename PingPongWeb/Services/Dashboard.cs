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
        private readonly IMatchService _matchService;

        public Dashboard(IPlayerService playerService, IMatchService matchService)
        {
            _playerService = playerService;
            _matchService = matchService;
        }



        public async Task<GroupViewModel> CreateGroupViewModel() //TODO create helper function to avoid duplication
        {
            List<string> groupA = await _playerService.GetSpecificGroupPlayers("A");
            List<string> groupB = await _playerService.GetSpecificGroupPlayers("B");
            List<string> groupC = await _playerService.GetSpecificGroupPlayers("C");
            GroupViewModel groupViewModel = new()
            {
                groupA = groupA,
                groupB = groupB,
                groupC = groupC
            };
            return groupViewModel;
        }

        public async Task<GroupMatchViewModel> CreateGroupMatchesViewModel()
        {
            List<MatchDTO> groupA = await _matchService.GetSpecificGroupMatches("A");
            List<MatchDTO> groupB = await _matchService.GetSpecificGroupMatches("B");
            List<MatchDTO> groupC = await _matchService.GetSpecificGroupMatches("C");
            GroupMatchViewModel groupMatchViewModel = new()
            {
                groupA = groupA,
                groupB = groupB,
                groupC = groupC
            };
            return groupMatchViewModel;
        }

        public async Task<GroupMatchViewModel> CreatePlayoffMatchesViewModel()
        {
            List<MatchDTO> groupA = await _matchService.GetSpecificGroupMatches("A");
            List<MatchDTO> groupB = await _matchService.GetSpecificGroupMatches("B");
            List<MatchDTO> groupC = await _matchService.GetSpecificGroupMatches("C");

            //add all to groupView model
            GroupMatchViewModel groupMatchViewModel = new()
            {
                groupA = groupA,
                groupB = groupB,
                groupC = groupC
            };
            return groupMatchViewModel;
        }

        public void CreateOrUpdateMatchesForPlayoffs()
        {
            bool playoffMatchesAlreadyInDB = checkIf20PlayoffMatchesCreatedInDB();

            foreach (var match in playoffMatchesTemplate)
            {
                int matchNumber = int.Parse(match[0]); //get match number e.g. 1
                string player1FullName = ValidatePlayersNameForPlayoffs(match[1]);
                string player2FullName = ValidatePlayersNameForPlayoffs(match[2]);
                Match matchesObj = new(player1FullName, player2FullName, matchNumber, "Playoff");
                if (!playoffMatchesAlreadyInDB)
                {
                    _db.Matches.Add(matchesObj);
                }
                else
                {
                    Match specificMatch = _db.Matches.ToList().Where(x => x.MatchNr == matchNumber).Single();
                    specificMatch.Player1 = player1FullName;
                    specificMatch.Player2 = player2FullName;
                    _db.Matches.Update(specificMatch);
                }
            }
            _db.SaveChanges();


            /// <summary>
            /// Checks if 20 playoff matches is already added to Matches DB
            /// </summary>
            public bool checkIf20PlayoffMatchesCreatedInDB()
            {
                var matchesObj = _db.Matches.ToList();
                bool playoffMatchesAlreadyInDB = matchesObj.Where(x => x.MatchType == "Playoff").Count() == 20;
                return playoffMatchesAlreadyInDB;
            }
        }
}

