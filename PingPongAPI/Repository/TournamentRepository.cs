using System;
using PingPongAPI.Data;
using PingPongAPI.Models;
using PingPongAPI.Repository.IRepository;

namespace PingPongAPI.Repository
{
	public class TournamentRepository : Repository<Tournament>, ITournamentRepository
	{
        private readonly ApplicationDbContext _db;

        public TournamentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Tournament> UpdateAsync(Tournament entity)
        {
            _db.Tournaments.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        
    }
}

