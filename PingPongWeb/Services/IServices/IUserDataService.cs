using System;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;

namespace PingPongWeb.Services.IServices
{
    public interface IUserDataService
    {
        Task<List<MatchDTO>> GetUserMatches(string fullName);
    }
}
