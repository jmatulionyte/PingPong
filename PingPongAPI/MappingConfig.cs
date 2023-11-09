using System;
using AutoMapper;
using PingPongAPI.Models;
using PingPongAPI.Models.Dto;

namespace PingPongAPI
{
	public class MappingConfig : Profile
	{
        public MappingConfig()
        {
            CreateMap<Player, PlayerDTO>().ReverseMap();
            CreateMap<Player, PlayerCreateDTO>().ReverseMap();
            CreateMap<Player, PlayerUpdateDTO>().ReverseMap();

            CreateMap<Match, MatchDTO>().ReverseMap();
            CreateMap<Match, MatchUpdateDTO>().ReverseMap();
            CreateMap<Match, MatchCreateDTO>().ReverseMap();

            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
	}
}

