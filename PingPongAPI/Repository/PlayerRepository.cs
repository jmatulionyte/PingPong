using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PingPongAPI.Data;
using PingPongAPI.Models;
using PingPongAPI.Models.Dto;
using PingPongAPI.Repository.IRepository;
using Utility;

namespace PingPongAPI.Repository
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
	{
        private readonly ApplicationDbContext _db;
        public PlayerRepository(ApplicationDbContext db, IMatchRepository matchRepo) : base(db)
        {
            _db = db;
        }

        public async Task<Player> UpdateAsync(Player entity)
        {
            _db.Players.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAllPlayersGroupPositionsWins()
        {
            //count how many wins for every player in group matches (Group DB) - > playerFullname, totalGroupWins
            List<PlayerWinCount> groupedByWinsCount = GetPlayersWinsInGroupMatches();

            if (groupedByWinsCount.Count != 0) //at least one group match winner exists
            {
                //loops A, B, C groups
                foreach (var groupName in SpecialDetails.Groups)
                {
                    //Get Players DB data (enrolled players to tournament and assigned to particullar group (A B C)
                    List<Player> playersInGroup = await GetAllAsync(p =>
                    p.GroupName == groupName && p.EnrolledToTournament == "Yes");

                    //Loop players DB and and set fullName, wins, groupName to groupResult DB
                    foreach (var player in playersInGroup)
                    {
                        //Find how many group matches specific player won
                        int groupWins = groupedByWinsCount.Where(x => x.Player == player.FirstName + " " + player.LastName)
                            .Select(x => x.PlayerWinsCount).SingleOrDefault();
                        //set data to groupResults DB
                        await SetPlayerGroupWinsToPlayersDB(player, groupWins);
                    }
                    await AssignGroupPosition(groupName);
                }
            }

        }

        /// <summary>
        /// Counts how many wins for every player in Group DB.
        /// Returns list of playerFullname and totalGroupWins
        /// </summary>
        private List<PlayerWinCount> GetPlayersWinsInGroupMatches()
        {
            List<Match> allMatches = _db.Matches.ToList();
            List<PlayerWinCount> playersWinCount = allMatches.Where(t => t.Winner != null && t.MatchType == "Group")
                .GroupBy(g => g.Winner)
                .Select(w => new PlayerWinCount
                {
                    Player = w.Key,
                    PlayerWinsCount = w.Distinct().Count()
                }).ToList();
            return playersWinCount;
        }

        /// <summary>
        /// Update players group wins int in Players DB
        /// </summary>
        private async Task SetPlayerGroupWinsToPlayersDB(Player player, int groupWins)
        {
            Player playerObj = await GetAsync(x => x.FirstName == player.FirstName && x.LastName == player.LastName);
            playerObj.GroupWins = groupWins;
            await UpdateAsync(playerObj);
        }

        /// <summary>
        ///  Get Players by groupName, order desc and assign positions (by wins) to players
        /// </summary>
        private async Task AssignGroupPosition(string groupName)
        {
            List<Player> playersInGroup = await GetAllAsync(p => p.GroupName == groupName && p.EnrolledToTournament == "Yes");
            var playersOrderedByWinsDesc = playersInGroup.OrderByDescending(t => t.GroupWins).Select(t => t).ToList();
            int positionCounter = 0;
            bool noMatchesPlayedInGroup = playersOrderedByWinsDesc.Where(x => x.GroupWins == 0).Count() == playersOrderedByWinsDesc.Count;
            foreach (var player in playersOrderedByWinsDesc)
            {
                //if after sorting all players in group
                //and expecting biggest score at the top and biggest score - 0
                //means no matches were played in the group
                //setting zero position to all players in group
                if (noMatchesPlayedInGroup)
                {
                    player.GroupPosition = 0;
                }
                else
                { // at least 1 match was played in group, so setting incremental positioning in group
                    positionCounter++;
                    player.GroupPosition = positionCounter;
                }
                await UpdateAsync(player);
            }
        }
    }
}

