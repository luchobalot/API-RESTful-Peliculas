﻿namespace API_Peliculas.Modelos.Dtos
{
    public class ActualizarPeliculaDto
    {

        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public int Duracion { get; set; }

        public string? RutaImagen { get; set; }

        public IFormFile Imagen { get; set; }

        public enum TipoClasificacion { Siete, Trece, Diesciseis, Dieciocho }

        public TipoClasificacion Clasificacion { get; set; }

        public int AnioEstreno { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public int categoriaId { get; set; }


    }
}
