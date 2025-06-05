
using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.Utilidades;

public class AutomapperProfile : Profile
{

    public AutomapperProfile()
    {
        CreateMap<Autor, AutoresDTO>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => MapearNombreCompleto(src)));

        CreateMap<Autor, AutorConLibrosDTO>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => MapearNombreCompleto(src)));

        CreateMap<AutorCreacionDTO, Autor>();
        CreateMap<Autor, AutorPatchDTO>().ReverseMap(); //Patch>


        CreateMap<Libro, LibroDTO>();
        CreateMap<LibroCreactionDTO, Libro>();

        // CreateMap<Libro, LibroConAutorDTO>()
        //     .ForMember(dest => dest.AutorNombre,
        //     opt => opt.MapFrom(src => MapearNombreCompleto(src.Autor!)));


        CreateMap<ComentarioCreationDTO, ComentarioDTO>();
        CreateMap<ComentarioCreationDTO, Comentario>();
        CreateMap<Comentario, ComentarioDTO>();

        CreateMap<ComentarioPatchDTO, Comentario>().ReverseMap();
    }

    private string MapearNombreCompleto(Autor autor)
    {
        return $"{autor.Nombres} {autor.Apellidos}";
    }

}
