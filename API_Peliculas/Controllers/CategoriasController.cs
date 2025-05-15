using API_Peliculas.Modelos;
using API_Peliculas.Modelos.Dtos;
using API_Peliculas.Repositorio.IRepositorio;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Plugins;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_Peliculas.Controllers
{
    [Route("api/[controller]")] // Define la ruta base para este controlador.
    [ApiController] // Este atributo activa características específicas para APIs web,
                    // como validación automática del modelo, inferencia de fuentes de parámetros, etc.
    public class CategoriasController : ControllerBase // Define la clase del controlador que hereda de ControllerBase
    {
        // Declaración de dependencias: necesitas un repositorio para acceder a los datos de
        // categorías y un mapper para transformar entre entidades y DTOs.
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;


        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            // Este es el constructor que utiliza inyección de dependencias. ASP.NET se encarga de
            // crear instancias de ICategoriaRepositorio y IMapper y pasarlas al controlador.
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        // ==========================================
        // |         VER TODAS LAS CATEOGRIAS       |
        // ==========================================

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // 403: Prohibido (el usuario no tiene permiso)
        [ProducesResponseType(StatusCodes.Status200OK)] // 200: OK (todo salió bien)
        public IActionResult GetCategorias()
        {
            var ListaCategorias = _ctRepo.GetCategorias();
            var ListaCategoriasDto = new List<CategoriaDto>();

            foreach (var lista in ListaCategorias)
            {
                ListaCategoriasDto.Add(_mapper.Map<CategoriaDto>(lista));
            }
            return Ok(ListaCategoriasDto);

            // Obtiene todas las categorías del repositorio
            // Crea una nueva lista para almacenar los DTOs
            // Convierte cada entidad de categoría a un DTO usando AutoMapper
            // Devuelve un código de estado 200(OK) con la lista de DTOs
        }

        // ==========================================
        // |           VER CATEGORIA POR ID         |
        // ==========================================

        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // 400: Solicitud incorrecta (datos inválidos)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // 404: No encontrado (la categoría solicitada no existe)
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _ctRepo.GetCategoria(categoriaId); // Llama al método GetCategoria del repo para buscar la categoría en la bdd
            if (itemCategoria == null)
            {
                return NotFound(); // Si no existe (es null), devuelve una respuesta 404 Not Found
            }

            // Convierte la entidad Categoria recuperada de la base de datos a un DTO CategoriaDto
            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);
            return Ok(itemCategoriaDto);
        }


        // ==========================================
        // |          CREAR NUEVA CATEGORIA         |
        // ==========================================

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // 201: Creado (éxito al crear un recurso)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // 500: Error interno del servidor
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // 401: No autorizado
        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto) // [FromBody] indica que los datos vendrán en el cuerpo de la petición HTTP
        {
            //Verifica si los datos recibidos cumplen con las validaciones definidas en tu DTO
            // Por ejemplo, si el nombre es requerido o tiene una longitud máxima
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica si el objeto recibido es nulo
            if (crearCategoriaDto == null)
            {
                return BadRequest(ModelState);
            }

            // Verifica si ya existe una categoría con el mismo nombre en la base de datos
            // Si existe, añade un mensaje de error al ModelState y devuelve un error
            if (_ctRepo.ExisteCategoria(crearCategoriaDto.Nombre))
            {
                ModelState.AddModelError("", $"Error. La categoria ya existe!");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto); // AutoMapper para convertir el DTO recibido en una entidad Categoria


            //Intenta guardar la nueva categoría en la base de datos usando tu repositorio
            // Si falla, añade un mensaje de error y devuelve un error
            if (!_ctRepo.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{categoria.Nombre}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }

        // ==========================================
        // |      ACTUALIZAR CATEGORIA - PATCH      |
        // ==========================================

        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoriaExistente = _ctRepo.GetCategoria(categoriaId);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontro la categoria con el ID {categoriaId}");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            //Intenta guardar la nueva categoría en la base de datos usando tu repositorio
            // Si falla, añade un mensaje de error y devuelve un error
            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // ==========================================
        // |          BORRAR UNA CATEGORIA          |
        // ==========================================

        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult BorrarCategoria(int categoriaId)
        {
            if (!_ctRepo.ExisteCategoria(categoriaId))
            {
                return NotFound($"No se encontro la categoría con el ID {categoriaId}");
            }

            var categoria = _ctRepo.GetCategoria(categoriaId);
            
            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}