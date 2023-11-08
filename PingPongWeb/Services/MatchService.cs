using System;
using Newtonsoft.Json.Linq;
using PingPongWeb.Services.IServices;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;
using Utility;
using Newtonsoft.Json;

namespace PingPongWeb.Services
{
	public class MatchService : BaseService, IMatchService
	{
        public readonly IHttpClientFactory _clientFactory;
        private string pingPongUrl;
        public MatchService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            pingPongUrl = configuration.GetValue<string>("ServiceUrls:PingPongAPI");
        }

        public List<string[]> playoffMatchesTemplate = new()
        {
        new string[3]{ "1", "B8", "C1"},
        new string[3]{ "2", "B7", "C2"},
        new string[3]{ "3", "B6", "C3" },
        new string[3]{ "4", "B5", "C4" },

        new string[3]{ "5", "B1", "Winner of 1" },
        new string[3]{ "6", "B2", "Winner of 2" },
        new string[3]{ "7", "B3", "Winner of 3" },
        new string[3]{ "8", "B4", "Winner of 4" },

        new string[3]{ "9", "A8", "Winner of 5" },
        new string[3]{ "10", "A7", "Winner of 6" },
        new string[3]{ "11", "A6", "Winner of 7" },
        new string[3]{ "12", "A5", "Winner of 8" },

        new string[3]{ "13", "A1", "Winner of 9" },
        new string[3]{ "14", "A2", "Winner of 10" },
        new string[3]{ "15", "A3", "Winner of 11" },
        new string[3]{ "16", "A4", "Winner of 12" },

        new string[3]{ "17", "Winner of 13", "Winner of 16" },
        new string[3]{ "18", "Winner of 15", "Winner of 14" },
        new string[3]{ "19", "Winner of 17", "Winner of 18" },
        new string[3]{ "20", "Loser of 17", "Loser of 18" }
        };

        public Task<T> CreateAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.POST,
                Url = pingPongUrl + "/api/MatchAPI",
                //Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string? groupName = "")
        {
            string query = "";
            if (groupName != null)
            {
                query = "?groupName=" + groupName;
            }
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.GET,
                Url = pingPongUrl + "/api/MatchAPI" + query
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.GET,
                Url = pingPongUrl + "/api/MatchAPI/" + id
            });
        }

        /// <summary>
        ///  Gets all matches belonging to specific group
        /// </summary>
        /// <param name="groupName"> string A, B or C
        public async Task<List<MatchDTO>> GetSpecificGroupMatches(string groupName)
        {
            var response = await GetAllAsync<APIResponse>(groupName);
            if (response != null && response.IsSuccess)
            {
                List<MatchDTO> group = JsonConvert.DeserializeObject<List<MatchDTO>>(Convert.ToString(response.Result));
                return group;
            }
            return new List<MatchDTO>();
        }

        /// <summary>
        ///  Gets all matches belonging to specific user
        /// </summary>
        /// <param name="userFullName">
        public async Task<List<MatchDTO>> GetGroupMatchesForUser(string userFullName)
        {
            var response = await GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                List<MatchDTO> group = JsonConvert.DeserializeObject<List<MatchDTO>>(Convert.ToString(response.Result));
                group = group.Where(u => u.Player1 == userFullName || u.Player2 == userFullName).Select(u => u).ToList();
                return group;
            }
            return new List<MatchDTO>();
        }

        public Task<T> UpdateAsync<T>(MatchUpdateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.PUT,
                Data = dto,
                Url = pingPongUrl + "/api/MatchAPI/" + dto.Id,
                Token = token
            });
        }


        /// <summary>
        ///  Create playoff matches and saves to Mathes DB IF they are not already created
        /// </summary>
        public void CreateOrUpdateMatchesForPlayoffs()
        {
            bool playoffMatchesAlreadyInDB = checkIf20PlayoffMatchesCreatedInDB();

            foreach (var match in playoffMatchesTemplate)
            {
                int playoffMatchNumber = int.Parse(match[0]); //get match number e.g. 1
                string player1FullName = ValidatePlayersNameForPlayoffs(match[1]);
                string player2FullName = ValidatePlayersNameForPlayoffs(match[2]);
                MatchCreateDTO matchesObj = new(player1FullName, player2FullName, playoffMatchNumber, "Playoff");
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
        }

        /// <summary>
        /// For Playoff Games, check if there are winners in matches between real players
        /// Winner would be assigned as a player for other match in a playoff
        /// </summary>
        public void GetWinnerOfMatchUpdateMatchDB()
        {
            List<Match> allPlayoffMatches = GetMatchesForPlayoffs();
            //MAYBE NEED TO CHECK IF THERE ARE PLAYER TO UPDATE TO!!!!!
            foreach (var match in allPlayoffMatches)
            {   //chech if there matches with winners
                if (match.Winner.Length != 0)
                {
                    SetPlayoffWinnersForFinalRounds(allPlayoffMatches, match);
                }
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Updates Matches DB with winner values. Values are seen in playoffs graph instead of "winner of X"
        /// </summary>
        private void SetPlayoffWinnersForFinalRounds(List<Match> allPlayoffMatches, Match match)
        {
            string matchIndicator = "of " + match.MatchNr.ToString();

            //find match which would provide winner value for player1
            List<Match> matchToSetWinnerForPlayer1 = allPlayoffMatches
                        .Where(x => x.Player1.EndsWith(matchIndicator) == true)
                        .Select(x => x).ToList();
            //find match which would provide winner value for player2
            List<Match> matchToSetWinnerForPlayer2 = allPlayoffMatches
                .Where(x => x.Player2.EndsWith(matchIndicator) == true)
                .Select(x => x).ToList();

            //if match was played and winner exists - update DB either for Player1 or Player2 value
            if (matchToSetWinnerForPlayer1.Count != 0)
            {
                matchToSetWinnerForPlayer1[0].Player1 = match.Winner;
                _db.Matches.Update(matchToSetWinnerForPlayer1[0]);
            }
            if (matchToSetWinnerForPlayer2.Count != 0)
            {
                matchToSetWinnerForPlayer2[0].Player2 = match.Winner;
                _db.Matches.Update(matchToSetWinnerForPlayer2[0]);
            }
        }

        /// <summary>
        ///  Create playoff matches and saves to Mathes DB IF they are not already created
        /// </summary>
        public List<Match> GetMatchesForPlayoffs()
        {
            List<Match> playoffMatchesObj = _db.Matches.Where(x => x.MatchType == "Playoff").ToList();
            return playoffMatchesObj;
        }

        /// <summary>
        /// Checks if player name is in positioning list, if not, assigns default positioning (e.g. A7)
        /// </summary>
        private string ValidatePlayersNameForPlayoffs(string playerPositioning)
        {
            string playerPositioningInGroup = playerPositioning;
            string playerGroup = playerPositioning[..1];

            string playerFullName = GetPlayersNameByHisPositioningInGroupDB(playerPositioningInGroup, playerGroup);
            if (playerFullName == null)
            {
                playerFullName = playerPositioningInGroup;
            }
            return playerFullName;
        }

        /// <summary>
        /// Checks if 20 playoff matches is already added to Matches DB
        /// </summary>
        private bool checkIf20PlayoffMatchesCreatedInDB()
        {
            var matchesObj = _db.Matches.ToList();
            bool playoffMatchesAlreadyInDB = matchesObj.Where(x => x.MatchType == "Playoff").Count() == 20;
            return playoffMatchesAlreadyInDB;
        }

        /// <summary>
        /// Get players name from GroupResults DB by positioning in group data (Could be null)
        /// </summary>
        public string GetPlayersNameByHisPositioningInGroupDB(string playersPositioningData, string groupName)
        {
            string? playerFullName = GetGroupResults()
                .Where(x => playersPositioningData.Contains(x.PositionInGroup.ToString()) == true && x.GroupName == groupName)
                .Select(x => x.PlayerFullName).SingleOrDefault();
            return playerFullName;
        }
    }
}

