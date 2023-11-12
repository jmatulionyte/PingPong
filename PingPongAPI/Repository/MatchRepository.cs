using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PingPongAPI.Data;
using PingPongAPI.Models;
using PingPongAPI.Models.Dto;
using PingPongAPI.Repository.IRepository;

namespace PingPongAPI.Repository
{
    public class MatchRepository : Repository<Match>, IMatchRepository
	{
        private readonly ApplicationDbContext _db;
        //dependency injection
        public MatchRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Match> UpdateAsync(Match entity)
        {
            _db.Matches.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task RemoveAllAsync()
        {
            var matches = _db.Matches;
            _db.Matches.RemoveRange(matches);
            await _db.SaveChangesAsync();
        }

        private readonly List<string[]> playoffMatchesTemplate = new()
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

        /// <summary>
        ///  Create playoff matches and saves to Mathes DB IF they are not already created
        /// </summary>
        public async Task CreateOrUpdateMatchesForPlayoffs()
        {
            bool playoffMatchesAlreadyInDB = await CheckIf20PlayoffMatchesCreatedInDB();

            foreach (var match in playoffMatchesTemplate)
            {
                int matchNumber = int.Parse(match[0]); //get match number e.g. 1
                string player1FullName = ValidatePlayersNameForPlayoffs(match[1]);
                string player2FullName = ValidatePlayersNameForPlayoffs(match[2]);
                Match matchesObj = new(player1FullName, player2FullName, matchNumber, "Playoff");
                if (!playoffMatchesAlreadyInDB)
                {
                    await CreateAsync(matchesObj);
                }
                else
                {
                    //get playoff match  - specific number (1-20)
                    Match specificMatch = (await GetAllAsync(x => x.PlayoffMatchNr == matchNumber)).Single();
                    specificMatch.Player1 = player1FullName;
                    specificMatch.Player2 = player2FullName;
                    await UpdateAsync(specificMatch);
                }
            }
        }

        /// <summary>
        /// For Playoff Games, check if there are winners in matches between players
        /// Winner would be assigned as a player for other match in a playoff
        /// </summary>
        public async Task GetWinnerOfMatchUpdateMatchDB()
        {
            List<Match> allPlayoffMatches = await GetAllAsync(x => x.MatchType == "Playoff");
            //MAYBE NEED TO CHECK IF THERE ARE PLAYER TO UPDATE TO!!!!!
            foreach (var match in allPlayoffMatches)
            {   //chech if there are matches with winners
                if (match.Winner.Length != 0)
                {
                    await SetPlayoffWinnersForFinalRounds(allPlayoffMatches, match);
                }
            }
        }

        /// <summary>
        /// Updates Matches DB - Playoff matches - with winner values. Values are seen in playoffs graph instead of "winner of X"
        /// </summary>
        private async Task SetPlayoffWinnersForFinalRounds(List<Match> allPlayoffMatches, Match match)
        {
            string matchIndicator = "of " + match.PlayoffMatchNr.ToString();

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
                await UpdateAsync(matchToSetWinnerForPlayer1[0]);
            }
            if (matchToSetWinnerForPlayer2.Count != 0)
            {
                matchToSetWinnerForPlayer2[0].Player2 = match.Winner;
                await UpdateAsync(matchToSetWinnerForPlayer2[0]);
            }
        }

        /// <summary>
        /// Checks if player has a specific position in group (e.g. A7),
        /// if yes - uses his name in playoffGraph
        /// if not, assigns default positioning (e.g. A7)
        /// </summary>
        private string ValidatePlayersNameForPlayoffs(string playerPositioning)
        {
            string playerPositioningInGroup = playerPositioning; //why like this
            string playerGroup = playerPositioning[..1];
            string playerForPosition = GetPlayersNameByHisPositioningInGroupDB(playerPositioningInGroup, playerGroup);
            if (playerForPosition!= null)
            {
                return playerForPosition;
            }
            return playerPositioningInGroup;
        }

        /// <summary>
        /// Checks if 20 playoff matches is already added to Matches DB
        /// </summary>
        private async Task<bool> CheckIf20PlayoffMatchesCreatedInDB()
        {
            return (await GetAllAsync(x => x.MatchType == "Playoff")).Count == 20;
        }

        /// <summary>
        /// Get players fullname from Player DB in specific group (A, B, C) by positioning in group (Could be null or e.g. A7)
        /// e.g. if players position is 7th in group A, then in playoffs he will be in A7 posotion
        /// </summary>
        private string GetPlayersNameByHisPositioningInGroupDB(string playersPositioningData, string groupName)
        {
            List<Player> players = _db.Players.ToList();
            //players = players.Where(x => playersPositioningData.Contains(
            //                x.GroupPosition.ToString()) == true && x.GroupName == groupName).ToList();
            //if (players != null && players.Count != 0)
            //{

            //    return players.Select(x => x.FirstName + " " + x.LastName).SingleOrDefault();


            //}

            return players
                .Where(x => playersPositioningData.Contains(x.GroupPosition.ToString()) == true && x.GroupName == groupName)
                .Select(x => x.FirstName + " " + x.LastName).SingleOrDefault();
        }
    }
}

