using API_Peliculas.Data;
using API_Peliculas.PeliculasMapper;
using API_Peliculas.Repositorio;
using API_Peliculas.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL")));

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")  // Puerto predeterminado de Vite
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();  // Para JWT, cookies, etc.
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Repositorios:
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Mapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));


var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

// AUTENTICACION y AUTORIZACION
builder.Services.AddAuthentication(
    x => {

        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Usa JWT Bearer para verificar quién es el usuario
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Usa JWT Bearer para manejar usuarios no autenticados
    }

    ).AddJwtBearer( x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Soporte de autenticaciòn

// Habilitar CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();