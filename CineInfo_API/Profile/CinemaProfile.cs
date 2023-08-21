using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Models;

namespace CineInfo_API.Profiles;

public class CinemaProfile : Profile {
    public CinemaProfile() {
        CreateMap<InputCinemaDTO, Cinema>();
        CreateMap<Cinema, InputCinemaDTO>();
        CreateMap<Cinema, ReadCinemaDTO>()
            .ForMember(
                cine => cine.Address,
                opt => opt.MapFrom(cine => cine.Address)
            );
    }
}
