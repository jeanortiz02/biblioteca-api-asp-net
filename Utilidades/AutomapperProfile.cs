
using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.Utilidades;

public class AutomapperProfile : Profile
{

    public AutomapperProfile()
    {
        CreateMap<Autor, AutoresDTO>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombres} {src.Apellidos}"));

        CreateMap<AutorCreacionDTO, Autor>();
    }

}
