using API_Peliculas.Modelos; // Se importan los modelos que utilizara la interfaz

namespace API_Peliculas.Repositorio.IRepositorio
{
    public interface ICategoriaRepositorio
    {
        ICollection<Categoria> GetCategorias(); // Método para obtener todas las categorías como una colección
        Categoria GetCategoria(int CategoriaId); // Método para obtener una categoría específica por su ID
        bool ExisteCategoria(int id); // Método para verificar si existe una categoría con el ID especificado

        bool ExisteCategoria(string nombre); // Sobrecarga del método anterior para verificar si existe una categoría con el nombre especificado

        bool CrearCategoria(Categoria categoria); // Método para crear una nueva categoría

        bool ActualizarCategoria(Categoria categoria); // Método para actualizar una categoría existente

        bool BorrarCategoria(Categoria categoria); // Método para eliminar una categoría

        bool Guardar(); // Método para guardar los cambios en la base de datos
    }
}
