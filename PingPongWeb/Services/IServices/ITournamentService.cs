using System;
using PingPongWeb.Models.Dto;

namespace PingPongWeb.Services.IServices
{
	public interface ITournamentService
	{
        Task<T> GetAllAsync<T>();
        //Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>();
        Task<T> UpdateAsync<T>(TournamentUpdateDTO dto);
        Task<bool> CheckIfOngoingTournament();
        Task EndTournament();
    }
}

