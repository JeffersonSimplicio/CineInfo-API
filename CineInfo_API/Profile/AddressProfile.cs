using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Models;

namespace CineInfo_API.Profiles;

public class AddressProfile : Profile {
    public AddressProfile() {
        CreateMap<CreateAddressDTO, Address>();
        CreateMap<UpdateAddressDTO, Address>();
        CreateMap<Address, UpdateAddressDTO>();
        CreateMap<Address, ReadAddressDTO>();
    }
}
