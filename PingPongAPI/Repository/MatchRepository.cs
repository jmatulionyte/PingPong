using System;
using System.Diagnostics;
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

        /// <summary>
        ///  Creates group plays with 2 players and 
        ///  populates Groups DB with this data for every play
        /// </summary>
        /// <param name="allPlayers"> List that hold every player in the group
        public async Task CreateMatchesForGroups(List<Player> allPlayers)
        {
            string[] groups = { "A", "B", "C" };
            foreach(var group in groups) //loop all the groups
            {
                //take players from specific group
                List<Player> playersInSpecificGroup = allPlayers.Where(x => x.GroupName == group).ToList(); 
                for (var i = 0; i < playersInSpecificGroup.Count; i++)
                {
                    var player = playersInSpecificGroup[i]; //take player
                    for (var j = i + 1; j < playersInSpecificGroup.Count; j++)//in every iterations, pair player with all other players
                    {
                        var nextPlayer = playersInSpecificGroup[j];
                        Match matchesObj = new(player.FirstName + " " + player.LastName,
                            nextPlayer.FirstName + " " + nextPlayer.LastName, group, "Group");
                        _db.Matches.Add(matchesObj);
                    }
                }
            }
            await _db.SaveChangesAsync();
        }

        public async Task<Match> UpdateAsync(Match entity)
        {
            _db.Matches.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        
    }
}

