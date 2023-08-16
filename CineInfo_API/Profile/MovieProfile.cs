using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Interfaces;
using CineInfo_API.Models;

namespace CineInfo_API.Profiles;

public class MovieProfile : Profile {
    public MovieProfile() {
        CreateMap<CreateMovieDTO, Movie>();
        CreateMap<UpdateMovieDTO, Movie>();
        CreateMap<Movie, UpdateMovieDTO>();
        CreateMap<Movie, ReadMovieDTO>();
    }
}
