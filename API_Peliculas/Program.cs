using API_Peliculas.Data;
using API_Peliculas.Modelos;
using API_Peliculas.PeliculasMapper;
using API_Peliculas.Repositorio;
using API_Peliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Configuración de servicios
// ----------------------------

// Conexión a la base de datos
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

// Agrega controladores
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
 
builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Autenticación JWT usando el esquema Bearer.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });

// Soporte para Autenticación .NET Identity
builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();



// Soporte para el versionamiento
var apiVersioningBuilder = builder.Services.AddApiVersioning(opcion =>
    {
    opcion.AssumeDefaultVersionWhenUnspecified = true;
    opcion.DefaultApiVersion = new ApiVersion(1, 0);
    opcion.ReportApiVersions = true;
        opcion.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version")
        );
});

apiVersioningBuilder.AddApiExplorer(
    opciones =>
    {
        opciones.GroupNameFormat = "'v'VVV";
    }

    );

// Repositorios
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Mapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

// Configuración de autenticación y JWT
var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// ----------------------------
// Configuración del pipeline
// ----------------------------
var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Soporte para archivos estáticos como imágenes
app.UseStaticFiles();


app.UseHttpsRedirection();

// Habilita CORS
app.UseCors("AllowReactApp");

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapea los controladores
app.MapControllers();

// Corre la aplicación
app.Run();
