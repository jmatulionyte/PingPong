using EntityFrameworkCore.Triggered;
using PingPongAPI.Models;
using PingPongAPI.Repository.IRepository;
using Utility;

namespace PingPongAPI.Repository
{
	public class SetPlayersGroupWinsPositions : IAfterSaveTrigger<Match>
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IPlayerRepository _playerRepo;
        //dependency injection
        public SetPlayersGroupWinsPositions(IMatchRepository matchRepo, IPlayerRepository playerRepo)
        {
            _matchRepo = matchRepo;
            _playerRepo = playerRepo;
        }

        public Task AfterSave(ITriggerContext<Match> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Modified && context.Entity.Winner != null)
            {
                //var task = Task.Run(async () => await UpdateAllPlayersGroupPositionsWins());
                //var result = task.WaitAndUnwrapException();

                //var result = AsyncContext.Run(UpdateAllPlayersGroupPositionsWins);

                //var result = AsyncContext.RunTask(UpdateAllPlayersGroupPositionsWins).Result;
                //await UpdateAllPlayersGroupPositionsWins();
                UpdateAllPlayersGroupPositionsWins().GetAwaiter().GetResult();

            }

            return Task.CompletedTask;
        }

        private async Task UpdateAllPlayersGroupPositionsWins()
        {
            //count how many wins for every player in group matches (Group DB) - > playerFullname, totalGroupWins
            List<PlayerWinCount> groupedByWinsCount = await GetPlayersWinsInGroupMatches();

            if (groupedByWinsCount.Count != 0) //at least one group match winner exists
            {
                //loops A, B, C groups
                foreach (var groupName in SpecialDetails.Groups)
                {
                    //Get Players DB data (enrolled players to tournament and assigned to particullar group (A B C)
                    List<Player> playersInGroup = await _playerRepo.GetAllAsync(p => p.GroupName == groupName && p.EnrolledToTournament == "Yes");

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
        private async Task<List<PlayerWinCount>> GetPlayersWinsInGroupMatches()
        {
            List<Match> allMatches = await _matchRepo.GetAllAsync();
            List<PlayerWinCount> playersWinCount = allMatches.Where(t => t.Winner != "")
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
            Player playerObj = await _playerRepo.GetAsync(x => x.FirstName == player.FirstName && x.LastName == player.LastName);
            playerObj.GroupWins = groupWins;
            await _playerRepo.UpdateAsync(playerObj);
        }

        /// <summary>
        ///  Get Players by groupName, order desc and assign positions (by wins) to players
        /// </summary>
        private async Task AssignGroupPosition(string groupName)
        {
            List<Player> playersInGroup = await _playerRepo.GetAllAsync(p => p.GroupName == groupName && p.EnrolledToTournament == "Yes");
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
                await _playerRepo.UpdateAsync(player);
            }
        }

        
    }
}

