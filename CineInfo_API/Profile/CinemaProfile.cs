using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Models;

namespace CineInfo_API.Profiles;

public class CinemaProfile : Profile {
    public CinemaProfile() {
        CreateMap<CreateCinemaDTO, Cinema>();
    }
}
