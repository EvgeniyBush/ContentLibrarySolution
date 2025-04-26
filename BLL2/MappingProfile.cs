#pragma warning disable 

using AutoMapper;
using BLL2.DTO;
using DAL2.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>().ReverseMap();
        CreateMap<Audio, AudioDto>().ReverseMap();
        CreateMap<Video, VideoDto>().ReverseMap();
        CreateMap<Document, DocumentDto>().ReverseMap();

        CreateMap<Storage, StorageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationName));
    }
}
