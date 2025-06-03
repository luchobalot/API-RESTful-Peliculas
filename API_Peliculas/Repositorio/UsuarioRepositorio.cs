using API_Peliculas.Data;
using API_Peliculas.Modelos;
using API_Peliculas.Modelos.Dtos;
using API_Peliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Peliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _bd;
        // Obtiene la clave secreta desde la configuración appsettings.json
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsuarioRepositorio(ApplicationDbContext bd, IConfiguration config, UserManager<AppUsuario> userManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _bd = bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        // Busca un usuario especifico por su ID, retorna NULL si no lo encuentra
        public AppUsuario GetUsuario(string usuarioId) 
        {
            return _bd.AppUsuario.FirstOrDefault(c => c.Id == usuarioId);
        }

        // Obtiene todos los usuarios ordenados alfabéticamente por nombre de usuario
        public ICollection<AppUsuario> GetUsuarios()
        {
            return _bd.AppUsuario.OrderBy(c => c.UserName).ToList();
        }

        // Verificar si un nombre de usuario ya está en uso antes de registrar uno nuevo.
        public bool IsUniqueUser(string usuario)
        {
            var usuarioBd = _bd.AppUsuario.FirstOrDefault(u => u.UserName == usuario);

            if (usuarioBd == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var usuario = _bd.AppUsuario.FirstOrDefault(
                u => u.UserName.ToLower() == usuarioLoginDto.NombreUsuario.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);

            // Validación si el usuario no existe
            if (usuario == null || isValid == false)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            // Si existe el usuario, entonces se ejecuta esta parte del codigo:
            var roles = await _userManager.GetRolesAsync(usuario);
            var manejandoToken = new JwtSecurityTokenHandler(); // proporciona métodos para crear, escribir y validar tokens JWT

            // Convierte una cadena (claveSecreta, que es la clave secreta para firmar el token) en un arreglo de bytes usando codificación ASCII.
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            // Define el subject del token, que es una colección de claims (afirmaciones) sobre el usuario. Aquí se crea un ClaimsIdentity con una lista de Claim.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Un claim que almacena el nombre del usuario (por ejemplo, "JuanPerez").
                    new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                    // Un claim que almacena el rol del usuario (por ejemplo, "Admin" o "Usuario").
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
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
                Usuario = _mapper.Map<UsuarioDatosDto>(usuario)
            };

            return usuarioLoginRespuestaDto;
        }

        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto) // Cambiar retorno a UsuarioDatosDto
        {
            AppUsuario usuario = new AppUsuario()
            {
                UserName = usuarioRegistroDto.NombreUsuario,
                Email = usuarioRegistroDto.NombreUsuario,
                NormalizedEmail = usuarioRegistroDto.NombreUsuario.ToUpper(),
                Nombre = usuarioRegistroDto.Nombre
            };


            var result = await _userManager.CreateAsync(usuario, usuarioRegistroDto.Password);

            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult()) // "Admin" con mayúscula
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Registrado"));
                }

                await _userManager.AddToRoleAsync(usuario, "Admin"); // "Admin" con mayúscula

                var usuarioReturn = _bd.AppUsuario.FirstOrDefault(
                    u => u.UserName == usuarioRegistroDto.NombreUsuario);

                return _mapper.Map<UsuarioDatosDto>(usuarioReturn);
            }

            return new UsuarioDatosDto(); // Constructor correcto
        }
    }
}