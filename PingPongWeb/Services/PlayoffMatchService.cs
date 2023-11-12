using System;
using Newtonsoft.Json.Linq;
using PingPongWeb.Services.IServices;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;
using Utility;
using Newtonsoft.Json;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace PingPongWeb.Services
{
    public class PlayoffMatchService : BaseService, IPlayoffMatchService
    {
        public readonly IHttpClientFactory _clientFactory;
        private string pingPongUrl;
        private readonly IMapper _mapper;
        IPlayerService _playerService;

        public PlayoffMatchService(IHttpClientFactory clientFactory, IConfiguration configuration, IMapper mapper, IPlayerService playerService) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            pingPongUrl = configuration.GetValue<string>("ServiceUrls:PingPongAPI");
            _mapper = mapper;
            _playerService = playerService;
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.GET,
                Url = pingPongUrl + "/api/MatchAPI"
            });
        }
        /// <summary>
        ///  Gets all matches belonging to specific user
        /// </summary>
        /// <param name="userFullName">
        public async Task<List<MatchDTO>> GetPlayoffMatchesForUser(string userFullName)
        {
            var response = await GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                List<MatchDTO> group = JsonConvert.DeserializeObject<List<MatchDTO>>(Convert.ToString(response.Result));
                group = group.Where(u => (u.Player1 == userFullName || u.Player2 == userFullName) && u.MatchType == "Playoff")
                    .Select(u => u).ToList();
                return group;
            }
            return new List<MatchDTO>();
        }
    }
}

