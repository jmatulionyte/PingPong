using System;
using EntityFrameworkCore.Triggered;
using PingPongAPI.Data;
using PingPongAPI.Models;
using PingPongAPI.Repository.IRepository;
using Utility;

namespace PingPongAPI.Repository
{
	public class CreateGroupMatches : IAfterSaveTrigger<Tournament>
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IPlayerRepository _playerRepo;

        public CreateGroupMatches(IMatchRepository matchRepo, IPlayerRepository playerRepo)
        {
            _matchRepo = matchRepo;
            _playerRepo = playerRepo;
        }
        public Task AfterSave(ITriggerContext<Tournament> context, CancellationToken cancellationToken)
        {
            //check if startdate is bigger than end date - indicates that tournament has started and helper tables (Matches) needs to be created
            if (context.ChangeType == ChangeType.Added && context.Entity.StartDate > context.Entity.EndDate)
            {
                CreateMatchesForGroups().GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }


        /// <summary>
        ///  Creates group plays with 2 players and populates Match DB with this data
        /// </summary>
        public async Task CreateMatchesForGroups()
        {
            //if group matches are already created, do nothing
            if ((await _matchRepo.GetAllAsync()).Count > 0)
            {
                return;
            }
            List<Player> allPlayers = await _playerRepo.GetAllAsync();
            foreach (var group in SpecialDetails.Groups) //loop all the groups
            {
                //take players from specific group
                List<Player> playersInSpecificGroup = allPlayers.Where(x => x.GroupName == group && x.EnrolledToTournament == "Yes").ToList();
                for (var i = 0; i < playersInSpecificGroup.Count; i++)
                {
                    var player = playersInSpecificGroup[i]; //take player
                    for (var j = i + 1; j < playersInSpecificGroup.Count; j++)//in every iterations, pair player with all other players
                    {
                        var nextPlayer = playersInSpecificGroup[j];
                        Match matchesObj = new(player.FirstName + " " + player.LastName,
                            nextPlayer.FirstName + " " + nextPlayer.LastName, group, "Group");
                        await _matchRepo.CreateAsync(matchesObj);
                    }
                }
            }
        }
    }
}

