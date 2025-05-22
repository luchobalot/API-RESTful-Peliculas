using API_Peliculas.Data;
using API_Peliculas.Modelos;
using API_Peliculas.Modelos.Dtos;
using API_Peliculas.Repositorio.IRepositorio;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace API_Peliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _bd;
        // Obtiene la clave secreta desde la configuración (appsettings.json
        private string claveSecreta;

        public UsuarioRepositorio(ApplicationDbContext bd, IConfiguration config)
        {
            _bd = bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }

        // Busca un usuario especifico por su ID, retorna NULL si no lo encuentra
        public Usuario GetUsuario(int usarioId)
  
        {
            return _bd.Usuario.FirstOrDefault(c => c.Id == usarioId);
        }

        // Obtiene todos los usuarios ordenados alfabéticamente por nombre de usuario
        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuario.OrderBy(c => c.NombreUsuario).ToList();
        }

        // Verificar si un nombre de usuario ya está en uso antes de registrar uno nuevo.
        public bool IsUniqueUser(string usuario)
        {
            var usuarioBd = _bd.Usuario.FirstOrDefault(u => u.NombreUsuario == usuario);

            if (usuarioBd == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            // Se encripta la contraseña ingresada con MD5
            var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);
            var usuario = _bd.Usuario.FirstOrDefault(
                u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                && u.Password == passwordEncriptado
                );

            // Validación si el usuario no existe
            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            // Si existe el usuario, entonces se ejecuta esta parte del codigo:
            var manejandoToken = new JwtSecurityTokenHandler(); // proporciona métodos para crear, escribir y validar tokens JWT

            // Convierte una cadena (claveSecreta, que es la clave secreta para firmar el token) en un arreglo de bytes usando codificación ASCII.
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            // Define el subject del token, que es una colección de claims (afirmaciones) sobre el usuario. Aquí se crea un ClaimsIdentity con una lista de Claim.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Un claim que almacena el nombre del usuario (por ejemplo, "JuanPerez").
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    // Un claim que almacena el rol del usuario (por ejemplo, "Admin" o "Usuario").
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Establece la fecha y hora de expiración del token.
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Configura las credenciales de firma del token, especificando la clave y el algoritmo de firma
            };

            // Llama al método CreateToken del objeto JwtSecurityTokenHandler para generar un objeto JwtSecurityToken basado en la configuración proporcionada en tokenDescriptor.
            var token = manejandoToken.CreateToken(tokenDescriptor);

            // Crea una nueva instancia de un objeto UsuarioLoginRespuestaDto, que es un DTO
            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                // Llama al método WriteToken del objeto JwtSecurityTokenHandler para convertir el objeto JwtSecurityToken
                Token = manejandoToken.WriteToken(token),
                // Asigna al objeto usuario a la propieda Usuario del DTO
                Usuario = usuario
            };

            return usuarioLoginRespuestaDto;
        }

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);

            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = passwordEncriptado,
                Nombre = usuarioRegistroDto.Nombre,
                Rol = usuarioRegistroDto.Rol
            };
            
            _bd.Usuario.Add(usuario);
            await _bd.SaveChangesAsync();
            usuario.Password = passwordEncriptado;

            return usuario;
        }

        // Metodo de encriptación de contraseña con MD5 (Se usa tanto en el acceso como registro)
        public static string obtenermd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }
    }
}
