using System;
using PingPongWeb.Models.Dto;

namespace PingPongWeb.Services.IServices
{
	public interface IPlayoffGraphService
	{
        Task<Dictionary<int, MatchDTO>> ConvertPlayoffMatchesDataForGraph();

        Task<string> GetPlayoffsWinner();

        Task<string> GetPlayoffs3rdPlace();
    }
}

