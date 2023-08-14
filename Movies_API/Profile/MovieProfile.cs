using AutoMapper;
using Movies_API.DTOs;
using Movies_API.Models;

namespace Movies_API.Profiles;

public class MovieProfile : Profile {
    public MovieProfile() {
        CreateMap<CreateMovieDTO, Movie>();
    }
}