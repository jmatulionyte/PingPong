using System;
using Microsoft.Extensions.Configuration;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;
using PingPongWeb.Services.IServices;
using Utility;

namespace PingPongWeb.Services
{
	public class AuthService : BaseService, IAuthService
	{
        public readonly IHttpClientFactory _clientFactory;
        private string pingPongUrl;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            pingPongUrl = configuration.GetValue<string>("ServiceUrls:PingPongAPI");

        }

        public Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.POST,
                Data = obj,
                Url = pingPongUrl + "/api/UsersAuth/login"
            });

        }

        public Task<T> RegisterAsync<T>(RegistrationRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.POST,
                Data = obj,
                Url = pingPongUrl + "/api/UsersAuth/register"
            });
        }

        public Task<T> GetUserData<T>(string id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.POST,
                Data = id,
                Url = pingPongUrl + "/api/UsersAuth/userId"
            });
        }
    }
}

