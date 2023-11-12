using System;
using EntityFrameworkCore.Triggered;
using PingPongAPI.Data;
using PingPongAPI.Models;
using PingPongAPI.Repository.IRepository;
using Utility;

namespace PingPongAPI.Repository
{
	public class DeleteAllMatches : IAfterSaveTrigger<Tournament>
    {
        private readonly IMatchRepository _matchRepo;

        public DeleteAllMatches(IMatchRepository matchRepo)
        {
            _matchRepo = matchRepo;
        }
        public Task AfterSave(ITriggerContext<Tournament> context, CancellationToken cancellationToken)
        {
            //check if endDate is bigger then startDate - indicates that tournament has ended - helper table (Matches) needs to be deleted
            if (context.ChangeType == ChangeType.Modified && context.Entity.StartDate < context.Entity.EndDate) 
            {
                DeleteMatchesAll().GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///  Deletes all matches - group and playoffs
        /// </summary>
        public async Task DeleteMatchesAll()
        {
            await _matchRepo.RemoveAllAsync();
        }

    }
}

