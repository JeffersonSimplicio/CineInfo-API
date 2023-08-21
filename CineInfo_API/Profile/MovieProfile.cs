using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Models;

namespace CineInfo_API.Profiles;

public class MovieProfile : Profile {
    public MovieProfile() {
        CreateMap<InputMovieDTO, Movie>();
        CreateMap<Movie, InputMovieDTO>();
        CreateMap<Movie, ReadMovieDTO>();
    }
}
