using AutoMapper;
using MultipleLegacySystems.VehicleService.Models;
using MultipleLegacySystems.VehicleService.Responses;

namespace MultipleLegacySystems.VehicleService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Vehicle, VehicleResponse>()
            .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src));
    }
}