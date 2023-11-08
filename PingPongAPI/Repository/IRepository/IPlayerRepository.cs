using System;
using System.Linq.Expressions;
using PingPongAPI.Models;
using PingPongAPI.Repository;
using PingPongAPI.Repository.IRepository;
using static System.Net.Mime.MediaTypeNames;

namespace PingPongAPI.Repository.IRepository
{
    public interface IPlayerRepository : IRepository<Player>
    {
		Task<Player> UpdateAsync(Player entity);
    }
}