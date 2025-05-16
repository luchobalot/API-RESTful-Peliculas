using API_Peliculas.Data;
using API_Peliculas.Modelos;
using API_Peliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;

namespace API_Peliculas.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly ApplicationDbContext _bd;

        public CategoriaRepositorio(ApplicationDbContext bd) {
            _bd = bd; // Se almacena esta referencia en el campo privado _bd para usarla en todos los métodos.
        }

        public bool ActualizarCategoria(Categoria categoria)
        {
            try
            {
                // Primero, desconectamos cualquier entidad existente con el mismo ID
                var entidadExistente = _bd.Categorias.Local.FirstOrDefault(c => c.Id == categoria.Id);
                if (entidadExistente != null)
                {
                    _bd.Entry(entidadExistente).State = EntityState.Detached;
                }

                // Ahora actualizamos la nueva entidad
                categoria.FechaCreacion = DateTime.Now;
                _bd.Entry(categoria).State = EntityState.Modified;

                return Guardar();
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error
                Console.WriteLine($"Error al actualizar categoría: {ex.Message}");
                return false;
            }
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            // Marca la entidad para ser eliminada
            // Guarda los cambios y retorna el resultado
            _bd.Categorias.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            // Establece la fecha de creación
            // Agrega la nueva categoría al contexto
            // Guarda los cambios y retorna el resultado
            categoria.FechaCreacion = DateTime.Now;
            _bd.Categorias.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            // Verifica si existe alguna categoría con el nombre especificado
            // Usa LINQ con el método Any() para verificar la existencia
            // Normaliza el nombre pasando a minúsculas y eliminando espacios en blanco
            bool valor = _bd.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre);
            return valor;
        }

        public bool ExisteCategoria(int id)
        {
            return _bd.Categorias.Any(a => a.Id == id); // Verifica si existe una categoría con el ID proporcionado
        }

        public Categoria GetCategoria(int CategoriaId)
        {
            // Obtiene una categoría específica por su ID
            // FirstOrDefault devuelve la primera coincidencia o null si no existe
            return _bd.Categorias.FirstOrDefault(c => c.Id == CategoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            // Obtiene todas las categorías ordenadas alfabéticamente por nombre
            // Convierte el resultado a una lista
            return _bd.Categorias.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            // Persiste los cambios en la base de datos
            // SaveChanges() devuelve el número de entidades modificadas
            // Retorna true si se guardaron los cambios correctamente(0 o más cambios)
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
