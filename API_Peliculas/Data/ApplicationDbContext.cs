using API_Peliculas.Modelos;
using Microsoft.EntityFrameworkCore;

namespace API_Peliculas.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Categoria> Categoria { get; set; }
    }
}
