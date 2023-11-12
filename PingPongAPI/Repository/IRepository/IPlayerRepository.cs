using PingPongAPI.Models;

namespace PingPongAPI.Repository.IRepository
{
    public interface IPlayerRepository : IRepository<Player>
    {
		Task<Player> UpdateAsync(Player entity);
        Task UpdateAllPlayersGroupPositionsWins();
    }
}