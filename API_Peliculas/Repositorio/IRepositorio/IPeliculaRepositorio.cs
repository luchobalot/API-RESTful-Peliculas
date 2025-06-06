﻿using API_Peliculas.Modelos;

namespace API_Peliculas.Repositorio.IRepositorio
{
    public interface IPeliculaRepositorio
    {
        ICollection<Pelicula> GetPeliculas();

        ICollection<Pelicula> GetPeliculasEnCategoria(int catId);
        IEnumerable<Pelicula> BuscarPelicula(string nombre);

        Pelicula GetPelicula(int peliculaId); 
        bool ExistePelicula(int id);

        bool ExistePelicula(string nombre); 

        bool CrearPelicula(Pelicula pelicula); 

        bool ActualizarPelicula(Pelicula pelicula); 

        bool BorrarPelicula(Pelicula pelicula); 

        bool Guardar();
    }
}
