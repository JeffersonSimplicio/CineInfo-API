using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Models;

namespace CineInfo_API.Profiles;

public class SessionProfile : Profile {
    public SessionProfile() {
        CreateMap<InputSessionDTO, Session>();
        CreateMap<Session, InputSessionDTO>();
        CreateMap<Session, ReadSessionDTO>();
    }
}
