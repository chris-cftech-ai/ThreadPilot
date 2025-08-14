using AutoMapper;
using ThreadPilot.InsuranceService.Models;
using ThreadPilot.InsuranceService.Responses;

namespace ThreadPilot.InsuranceService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PersonInsuranceDetails, InsuranceResponse>()
            .ForMember(dest => dest.PersonInsuranceDetails, opt => opt.MapFrom(src => src));
        CreateMap<VehicleInfo, VehicleServiceResponse>()
            .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src));
    }
}