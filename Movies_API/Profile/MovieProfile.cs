using AutoMapper;
using Movies_API.DTOs;
using Movies_API.Interfaces;
using Movies_API.Models;

namespace Movies_API.Profiles;

public class MovieProfile : Profile {
    public MovieProfile() {
        CreateMap<CreateMovieDTO, Movie>();
        CreateMap<UpdateMovieDTO, Movie>();
        CreateMap<Movie, UpdateMovieDTO>();
    }
}