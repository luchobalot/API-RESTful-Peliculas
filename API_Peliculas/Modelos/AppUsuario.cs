using Microsoft.AspNetCore.Identity;

namespace API_Peliculas.Modelos
{
    public class AppUsuario : IdentityUser
    {
        public string Nombre { get; set; }
    }
}
