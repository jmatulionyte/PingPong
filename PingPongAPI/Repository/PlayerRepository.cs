﻿using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PingPongAPI.Data;
using PingPongAPI.Models;
using PingPongAPI.Models.Dto;
using PingPongAPI.Repository.IRepository;
using Utility;

namespace PingPongAPI.Repository
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
	{
        private readonly ApplicationDbContext _db;
        private readonly IMatchRepository _matchRepo;
        //dependency injection
        public PlayerRepository(ApplicationDbContext db, IMatchRepository matchRepo) : base(db)
        {
            _db = db;
            _matchRepo = matchRepo;
        }

        public async Task<Player> UpdateAsync(Player entity)
        {
            _db.Players.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}

