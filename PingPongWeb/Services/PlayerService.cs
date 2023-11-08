using System;
using Newtonsoft.Json.Linq;
using PingPongWeb.Services.IServices;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;
using Utility;
using Newtonsoft.Json;

namespace PingPongWeb.Services
{
	public class PlayerService : BaseService, IPlayerService
	{
        public readonly IHttpClientFactory _clientFactory;
        private string pingPongUrl;
        //provide dependency incection to base class and to this current class?
        //playerUrl from appsettings
        public PlayerService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            pingPongUrl = configuration.GetValue<string>("ServiceUrls:PingPongAPI");
        }

        public Task<T> CreateAsync<T>(PlayerCreateDTO dto)
        {
            dto.EnrolledToTournament = "No";
            dto.GroupName = "C";
            //sendAsync from base service
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.POST,
                Data = dto,
                Url = pingPongUrl + "/api/playerAPI"
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.DELETE,
                Url = pingPongUrl + "/api/playerAPI/" + id,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string? groupName = "")
        {
            string query = "";
            if(groupName != "")
            {
                query = "?groupName=" + groupName;
            }
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.GET,
                Url = pingPongUrl + "/api/playerAPI" + query
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.GET,
                Url = pingPongUrl + "/api/playerAPI/" + id,
            });
        }

        /// <summary>
        ///  Gets all players belonging to specific group
        /// </summary>
        /// <param name="groupName"> string A, B or C
        public async Task<List<string>> GetSpecificGroupPlayers(string groupName)
        {
            var response = await GetAllAsync<APIResponse>(groupName);
            if (response != null && response.IsSuccess)
            {
                List<PlayerDTO> group = JsonConvert.DeserializeObject<List<PlayerDTO>>(Convert.ToString(response.Result));
                return group.Select(u => u.FirstName + " " + u.LastName).ToList();
            }
            return new List<string>();
        }

        public Task<T> UpdateAsync<T>(PlayerUpdateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.PUT,
                Data = dto,
                Url = pingPongUrl + "/api/playerAPI/" + dto.Id,
                Token = token
            });
        }
    }
}

