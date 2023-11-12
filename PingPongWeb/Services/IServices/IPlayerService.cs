using PingPongWeb.Models.Dto;

namespace PingPongWeb.Services.IServices
{
    public interface IPlayerService
    {
        Task<T> GetAllAsync<T>(string? groupName = "");
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(PlayerCreateDTO dto);
        Task<T> UpdatePositionsWinsAsync<T>();
        Task<T> UpdatePlayerAsync<T>(PlayerUpdateDTO dto, string token);
        Task<T> DeleteAsync<T>(int id, string token);
        Task <List<string>> GetSpecificGroupPlayers(string groupName);
    }
}
