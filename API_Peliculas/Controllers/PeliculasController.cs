﻿using API_Peliculas.Modelos;
using API_Peliculas.Modelos.Dtos;
using API_Peliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Peliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepositorio _pelRepo;
        private readonly IMapper _mapper;


        public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper)
        {
            _pelRepo = pelRepo;
            _mapper = mapper;
        }

        // ==========================================
        // |        VER TODAS LAS PELICULAS         |
        // ==========================================

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {
            var ListaPeliculas = _pelRepo.GetPeliculas();
            var ListaPeliculasDto = new List<PeliculaDto>();

            foreach (var lista in ListaPeliculas)
            {
                ListaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            return Ok(ListaPeliculasDto);
        }


        // ==========================================
        // |           VER PELICULA POR ID          |
        // ==========================================

        [HttpGet("{peliculaId:int}", Name = "GetPelicula")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _pelRepo.GetPelicula(peliculaId); 
            if (itemPelicula == null)
            {
                return NotFound(); 
            }

         
            var itemPeliculaDto = _mapper.Map<PeliculaDto>(itemPelicula);
            return Ok(itemPeliculaDto);
        }


        // ==========================================
        // |          CREAR NUEVA PELICULA          |
        // ==========================================

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearPelicula([FromBody] CrearPeliculaDto crearPeliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            if (crearPeliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_pelRepo.ExistePelicula(crearPeliculaDto.Nombre))
            {
                ModelState.AddModelError("", $"Error. La pelicula ya existe!");
                return StatusCode(404, ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(crearPeliculaDto);

            if (!_pelRepo.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{pelicula.Nombre}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
        }

        // ==========================================
        // |      ACTUALIZAR CATEGORIA - PATCH      |
        // ==========================================

        [HttpPatch("{peliculaId:int}", Name = "ActualizarPatchPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchPelicula(int peliculaId, [FromBody] PeliculaDto peliculaDto)
        {
            if (!ModelState.IsValid || peliculaDto == null || peliculaId != peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            // Verificar si existe sin traer la entidad
            if (!_pelRepo.ExistePelicula(peliculaId))
            {
                return NotFound($"No se encontró la película con el ID {peliculaId}");
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        // ==========================================
        // |          BORRAR UNA PELICULA           |
        // ==========================================

        [HttpDelete("{peliculaId:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult BorrarPelicula(int peliculaId)
        {
            if (!_pelRepo.ExistePelicula(peliculaId))
            {
                return NotFound($"No se encontro la pelicula con el ID {peliculaId}");
            }

            var pelicula = _pelRepo.GetPelicula(peliculaId);

            if (!_pelRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        // ==========================================
        // |         PELICULAS EN CATEGORIA         |
        // ==========================================

        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPeliculas = _pelRepo.GetPeliculasEnCategoria(categoriaId);

            if (listaPeliculas == null)
            {
                return NotFound();
            }

            var itemPelicula = new List<PeliculaDto>();

            foreach (var pelicula in listaPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(pelicula));
            }

            return Ok(itemPelicula);
        }

        // ==========================================
        // |         PELICULAS EN CATEGORIA         |
        // ==========================================

        [HttpGet("Buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult Busar(string nombre)
        {
            try
            {
                var resultado = _pelRepo.BuscarPelicula(nombre);

                if (resultado.Any())
                {
                    return Ok(resultado);
                }

                return NotFound();

            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error recuperando los datos del servidor!");
            }
        }

    }
}