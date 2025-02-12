using System.ComponentModel.DataAnnotations;

namespace ApiPracticaBiblioteca.Models
{
    public class Autor
    {
        [Key]
        public int id_autor { get; set; }
        public string nombre { get; set; }

        public string nacionalidad { get; set; }


    }
}
