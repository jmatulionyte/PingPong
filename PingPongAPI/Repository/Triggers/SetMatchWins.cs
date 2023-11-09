using System;
using EntityFrameworkCore.Triggered;
using PingPongAPI.Data;
using PingPongAPI.Models;
using PingPongAPI.Repository.IRepository;

namespace PingPongAPI.Repository
{
	public class SetMatchWins : IBeforeSaveTrigger<Match>
    {
        public Task BeforeSave(ITriggerContext<Match> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified && context.Entity.Winner == "")
            {
                if (context.Entity.ResultPlayer1 > context.Entity.ResultPlayer2)
                {
                    context.Entity.Winner = context.Entity.Player1;
                }
                else if (context.Entity.ResultPlayer1 < context.Entity.ResultPlayer2)
                {
                    context.Entity.Winner = context.Entity.Player2;
                }
            }

            return Task.CompletedTask;
        }
    }
}

