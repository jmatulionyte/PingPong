using System;
using EntityFrameworkCore.Triggered;
using PingPongAPI.Data;
using PingPongAPI.Models;
using PingPongAPI.Repository.IRepository;
using Utility;

namespace PingPongAPI.Repository
{
	public class DeleteGroupPositionsWins : IAfterSaveTrigger<Tournament>
    {
        private readonly IPlayerRepository _playerRepo;

        public DeleteGroupPositionsWins(IPlayerRepository playerRepo)
        {
            _playerRepo = playerRepo;
        }
        public Task AfterSave(ITriggerContext<Tournament> context, CancellationToken cancellationToken)
        {
            //check if endDate is bigger then startDate - indicates that tournament has ended - helper table (Matches) needs to be deleted
            if (context.ChangeType == ChangeType.Modified && context.Entity.StartDate < context.Entity.EndDate) 
            {
                DeletePositionsWinsInPlayersDB().GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///  Deletes all matches - group and playoffs
        /// </summary>
        public async Task DeletePositionsWinsInPlayersDB()
        {
            List<Player> allPlayers = await _playerRepo.GetAllAsync();
            foreach(var player in allPlayers)
            {
                player.GroupPosition = 0;
                player.GroupWins = 0;
                await _playerRepo.UpdateAsync(player);
            }
        }

    }
}

