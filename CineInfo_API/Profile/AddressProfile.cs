using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Models;

namespace CineInfo_API.Profiles;

public class AddressProfile : Profile {
    public AddressProfile() {
        CreateMap<InputAddressDTO, Address>();
        CreateMap<Address, InputAddressDTO>();
        CreateMap<Address, ReadAddressDTO>();
    }
}
