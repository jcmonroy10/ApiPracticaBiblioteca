using System.ComponentModel.DataAnnotations;

namespace ApiPracticaBiblioteca.Models
{
    public class Libro
    {
        [Key]
        public int id_libro{ get; set; }
        public string titulo { get; set; }
        public int? anioPublicacion { get; set; }
        public int? id_autor {  get; set; }
        public int? id_categoria { get; set; }
        public string resumen {  get; set; }
    }
}
