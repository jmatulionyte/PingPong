using PingPongWeb.Models.Dto;

namespace PingPongWeb.Services.IServices
{
    public interface IMatchService
    {
        Task<T> GetAllAsync<T>(string? groupName = "");
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>();
        Task<T> UpdateAsync<T>(MatchUpdateDTO dto, string token);
        Task<List<MatchDTO>> GetSpecificGroupMatches(string groupName);
        Task<List<MatchDTO>> GetGroupMatchesForUser(string userFullName);
    }
}
