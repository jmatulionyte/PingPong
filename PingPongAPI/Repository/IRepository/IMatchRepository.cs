using PingPongAPI.Models;

namespace PingPongAPI.Repository.IRepository
{
    public interface IMatchRepository : IRepository<Match>
    {
		Task<Match> UpdateAsync(Match entity);

        Task CreateMatchesForGroups(List<Player> allPlayers);
    }
}