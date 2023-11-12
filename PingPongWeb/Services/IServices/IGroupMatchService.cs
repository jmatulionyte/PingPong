using PingPongWeb.Models.Dto;

namespace PingPongWeb.Services.IServices
{
    public interface IGroupMatchService
    {
        Task<T> GetAllAsync<T>(string? groupName = "");
        Task<T> GetAsync<T>(int id);
        Task<T> CreatePlayoffMatchAsync<T>(MatchCreateDTO dto);
        Task<T> CreateGroupMatchesAsync<T>();
        Task<T> UpdateAsync<T>(MatchUpdateDTO dto);
        Task<List<MatchDTO>> GetSpecificGroupMatches(string groupName);
        Task<List<MatchDTO>> GetGroupMatchesForUser(string userFullName);
        string GetWinnerFromMatch(MatchUpdateDTO matchObj);
    }
}
