using System;
using Newtonsoft.Json;
using PingPongWeb.Models;
using PingPongWeb.Models.Dto;
using PingPongWeb.Services;
using PingPongWeb.Services.IServices;
using Utility;

namespace PingPongWeb.Services
{
	public class TournamentService : BaseService, ITournamentService
	{
        public readonly IHttpClientFactory _clientFactory;
        private string pingPongUrl;
        //provide dependency incection to base class and to this current class?
        //playerUrl from appsettings
        public TournamentService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            pingPongUrl = configuration.GetValue<string>("ServiceUrls:PingPongAPI");
        }

        public Task<T> CreateAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.POST,
                Url = pingPongUrl + "/api/TournamentAPI"
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.GET,
                Url = pingPongUrl + "/api/TournamentAPI"
            });
        }

        public Task<T> UpdateAsync<T>(TournamentUpdateDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SpecialDetails.ApiType.PUT,
                Data = dto,
                Url = pingPongUrl + "/api/TournamentAPI/" + dto.Id,

            });
        }

        /// <summary>
        /// Checks if thera are reconrds in DB, where StartDate exists and end date is minimum date
        /// </summary>
        public async Task<bool> CheckIfOngoingTournament()
        {
            var response = await GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                List<TournamentDTO> tournamentList = JsonConvert.DeserializeObject<List<TournamentDTO>>(Convert.ToString(response.Result));
                return tournamentList.Any(u => u.EndDate < u.StartDate);
            }
            return false;
        }

        /// <summary>
        /// Checks if there are reconrds in DB, where StartDate exists and end date is minimum date.
        /// The update that record - updates Enddate to NOW
        /// </summary>
        public async Task EndTournament()
        {
            var response = await GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                List<TournamentUpdateDTO> tournamentList = JsonConvert.DeserializeObject<List<TournamentUpdateDTO>>(Convert.ToString(response.Result));
                //look for tournament where enddate hasnt been set (=end date equal erliest possible data = lower than startdate)
                TournamentUpdateDTO tournament = tournamentList.Where(u => u.EndDate < u.StartDate).FirstOrDefault();
                if(tournament != null) // only end tournamet that had been started
                {
                    tournament.EndDate = DateTime.Now;
                    response = await UpdateAsync<APIResponse>(tournament);
                    if (response != null && response.IsSuccess)
                    {
                        //??
                    }
                }
                
                //???
            }
        }
    }
}

