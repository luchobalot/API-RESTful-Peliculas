﻿using System.ComponentModel.DataAnnotations;

namespace API_Peliculas.Modelos.Dtos
{
    public class CrearCategoriaDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El número máximo de caracteres es de 100.")]
        
        public string Nombre { get; set; }
    }
}
