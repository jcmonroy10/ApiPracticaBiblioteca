using Microsoft.EntityFrameworkCore;

namespace ApiPracticaBiblioteca.Models
{
    public class bibliotecaContext: DbContext
    {
        public bibliotecaContext(DbContextOptions<bibliotecaContext> options) : base(options)
        {

        }

        public DbSet<Libro> Libro { get; set; }
        public DbSet<Autor> Autor { get; set; }
    }
}
