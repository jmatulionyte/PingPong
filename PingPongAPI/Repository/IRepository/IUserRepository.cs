using PingPongAPI.Models;
using PingPongAPI.Models.Dto;

namespace PingPongAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool isUserUnique(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegistrationRequestDTO registerationRequestDTO);
        string GetNameFromTokenData(string id);
    }
}