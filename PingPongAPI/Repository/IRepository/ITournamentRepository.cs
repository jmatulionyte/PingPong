using System;
using PingPongAPI.Models;

namespace PingPongAPI.Repository.IRepository
{
	public interface ITournamentRepository : IRepository<Tournament>
    {
        Task<Tournament> UpdateAsync(Tournament entity);
    }
}

