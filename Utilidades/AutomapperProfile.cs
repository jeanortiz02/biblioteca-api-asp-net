
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

        CreateMap<AutorLibro, LibroDTO>()
            .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.LibroId))
            .ForMember(dto => dto.Titulo, config => config.MapFrom(ent => ent.Libro!.Titulo));


        // Here
        // CreateMap<Autor, AutorConLibrosDTO>()
        //     .ForMember(
        //         dest => dest.Libros,
        //         opt => opt.MapFrom(src => src.Libros.Select(al => al.Libro))
        //     );
        CreateMap<Libro, LibroDTO>();
        CreateMap<LibroCreactionDTO, Libro>()
            .ForMember(ent => ent.Autores,
                config => config.MapFrom(dto => dto.AutoresId.Select(id => new AutorLibro { AutorId = id }))
            );

        CreateMap<Libro, LibroConAutoresDTO>();

        CreateMap<AutorLibro, AutoresDTO>()
            .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.AutorId))
            .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(ent => MapearNombreCompleto(ent.Autor!)));

        CreateMap<LibroCreactionDTO, AutorLibro>()
            .ForMember(ent => ent.Libro,
                config => config.MapFrom(dto => new Libro { Titulo = dto.Titulo }));



        CreateMap<ComentarioCreationDTO, ComentarioDTO>();
        CreateMap<ComentarioCreationDTO, Comentario>();
        CreateMap<Comentario, ComentarioDTO>()
            .ForMember(dto => dto.UsuarioEmail, confg => confg.MapFrom(ent => ent.Usuario!.Email));

        CreateMap<ComentarioPatchDTO, Comentario>().ReverseMap();
        

        CreateMap<Usuario, UsuarioDTO>();
    }

    private string MapearNombreCompleto(Autor autor)
    {
        return $"{autor.Nombres} {autor.Apellidos}";
    }

}
