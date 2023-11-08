using AutoMapper;
using PingPongWeb.Models.Dto;

namespace PingPongWeb
{
	public class MappingConfig : Profile
	{
        public MappingConfig()
        {
            CreateMap<PlayerDTO, PlayerCreateDTO>().ReverseMap();
            CreateMap<PlayerDTO, PlayerUpdateDTO>().ReverseMap();
            CreateMap<MatchDTO, MatchUpdateDTO>().ReverseMap();

        }
    }
}

