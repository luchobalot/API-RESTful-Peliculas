﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Peliculas.Modelos
{
    public class Pelicula
    {
        [Key]
        public  int Id { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public int Duracion { get; set; }

        public string? RutaImagen { get; set; }

        public string? RutaLocalImagen { get; set; }

        public enum TipoClasificacion { Siete, Trece, Diesciseis, Dieciocho}

        public TipoClasificacion Clasificacion {  get; set; }

        public int AnioEstreno { get; set; }

        public DateTime FechaCreacion { get; set; }


        public int categoriaId { get; set; }
        [ForeignKey("categoriaId")]
        public Categoria  Categoria { get; set; }

    }
}
