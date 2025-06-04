using API_Peliculas.Modelos;
using API_Peliculas.Modelos.Dtos;
using AutoMapper;

namespace API_Peliculas.PeliculasMapper
{
    public class PeliculasMapper : Profile
    {
        public PeliculasMapper() {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();

            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Pelicula, CrearPeliculaDto>().ReverseMap();
            CreateMap<Pelicula, ActualizarPeliculaDto>().ReverseMap();

            CreateMap<AppUsuario, UsuarioDatosDto>().ReverseMap();
        }

    }
}
