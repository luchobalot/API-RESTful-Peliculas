using System.ComponentModel.DataAnnotations;

namespace API_Peliculas.Modelos.Dtos
{
    public class UsuarioRegistroDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; }
    }
}
