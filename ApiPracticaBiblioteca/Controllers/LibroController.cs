using ApiPracticaBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPracticaBiblioteca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly bibliotecaContext _bibliotecaContexto;

        public LibroController(bibliotecaContext bibliotecaContexto)
        {
            _bibliotecaContexto = bibliotecaContexto;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Libro> listadoLibro = (from l in _bibliotecaContexto.Libro select l).ToList();

            if (listadoLibro.Count == 0)
            {
                return NotFound();
            }

            return Ok(listadoLibro);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var libro = (from l in _bibliotecaContexto.Libro
                         join a in _bibliotecaContexto.Autor
                         on l.id_autor equals a.id_autor
                         where l.id_libro == id
                         select new
                         {
                             l.id_libro,
                             l.titulo,
                             l.anioPublicacion,
                             l.id_categoria,
                             l.resumen,
                             l.id_autor,
                             autor = a.nombre,
                         }).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }

            return Ok(libro);
        }

        //Buscar por año de publicación
        [HttpGet]
        [Route("FindByAnio/{anio}")]
        public IActionResult FindByAnio(int anio)
        {
            var libros = _bibliotecaContexto.Libro
                .Where(l => l.anioPublicacion >= anio)
                .ToList();

            if (libros == null || libros.Count == 0)
            {
                return NotFound("No se encontraron libros publicados desde ese año en adelante.");
            }

            return Ok(libros);
        }

        //Buscar por titulo del libro
        [HttpGet]
        [Route("FindByTitle/{titulo}")]
        public IActionResult FindByTitle(string titulo)
        {
            var libros = _bibliotecaContexto.Libro
                .Where(l => l.titulo.Contains(titulo))
                .ToList();

            if (libros == null || libros.Count == 0)
            {
                return NotFound("No se encontraron libros con ese título.");
            }

            return Ok(libros);
        }

        //Buscar por autor
        [HttpGet]
        [Route("CountByAuthor/{nombreAutor}")]
        public IActionResult CountByAuthor(string nombreAutor)
        {
            var autor = _bibliotecaContexto.Autor
                .FirstOrDefault(a => a.nombre == nombreAutor);

            if (autor == null)
            {
                return NotFound($"No se encontró un autor con el nombre '{nombreAutor}'.");
            }

            int cantidadLibros = _bibliotecaContexto.Libro
                .Count(l => l.id_autor == autor.id_autor);

            if (cantidadLibros == 0)
            {
                return NotFound($"El autor '{nombreAutor}' no tiene libros registrados.");
            }

            return Ok(new { Autor = nombreAutor, CantidadDeLibros = cantidadLibros });
        }

        //OTRAS CONSULTAS

        //Obtener los Autores con Más Libros Publicados
        [HttpGet]
        [Route("autores-mas-libros")]
        public IActionResult ObtenerAutoresConMasLibros()
        {
            var autores = _bibliotecaContexto.Autor
                .Select(a => new
                {
                    a.nombre,
                    CantidadLibros = _bibliotecaContexto.Libro.Count(l => l.id_autor == a.id_autor)
                })
                .OrderByDescending(a => a.CantidadLibros)
                .ToList();

            return Ok(autores);
        }

        //Obtener los Libros Más Recientes
        [HttpGet]
        [Route("libros-recientes")]
        public IActionResult ObtenerLibrosMasRecientes()
        {
            var libros = _bibliotecaContexto.Libro
                .OrderByDescending(l => l.anioPublicacion)
                .Take(10) // Obtener los 10 más recientes
                .ToList();

            return Ok(libros);
        }

        //Cantidad Total de Libros por Año
        [HttpGet]
        [Route("cantidad-libros-por-anio")]
        public IActionResult CantidadTotalDeLibrosPorAnio()
        {
            var librosPorAnio = _bibliotecaContexto.Libro
                .GroupBy(l => l.anioPublicacion)
                .Select(g => new { Anio = g.Key, Cantidad = g.Count() })
                .OrderByDescending(l => l.Anio)
                .ToList();

            return Ok(librosPorAnio);
        }

        //Verificar si un Autor Tiene Libros Publicados
        [HttpGet]
        [Route("autor-tiene-libros/{idAutor}")]
        public IActionResult VerificarSiAutorTieneLibros(int idAutor)
        {
            bool tieneLibros = _bibliotecaContexto.Libro.Any(l => l.id_autor == idAutor);

            if (tieneLibros)
            {
                return Ok(new { AutorId = idAutor, Mensaje = "El autor tiene libros publicados." });
            }
            else
            {
                return Ok(new { AutorId = idAutor, Mensaje = "El autor no tiene libros publicados." });
            }
        }


        //Obtener el Primer Libro Publicado de un Autor
        [HttpGet]
        [Route("primer-libro/{idAutor}")]
        public IActionResult ObtenerPrimerLibroDeAutor(int idAutor)
        {
            var primerLibro = _bibliotecaContexto.Libro
                .Where(l => l.id_autor == idAutor)
                .OrderBy(l => l.anioPublicacion)
                .FirstOrDefault();

            if (primerLibro == null)
                return NotFound("El autor no tiene libros registrados.");

            return Ok(primerLibro);
        }


        //Crear
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarLibro([FromBody] Libro libro)
        {
            try
            {
                _bibliotecaContexto.Libro.Add(libro);
                _bibliotecaContexto.SaveChanges();
                return Ok(libro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Actualizar
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarLibro(int id, [FromBody] Libro libroModificar)
        {
            Libro? libroActual = (from l in _bibliotecaContexto.Libro where l.id_libro == id select l).FirstOrDefault();

            if (libroActual == null)
            {
                return NotFound();
            }

            libroActual.titulo = libroModificar.titulo;
            libroActual.anioPublicacion = libroModificar.anioPublicacion;
            libroActual.id_autor = libroActual.id_autor;
            libroActual.id_categoria = libroActual.id_categoria;
            libroActual.resumen = libroModificar.resumen;

            _bibliotecaContexto.Entry(libroActual).State = EntityState.Modified;
            _bibliotecaContexto.SaveChanges();

            return Ok(libroModificar);
        }

        //Eliminar
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarLibro(int id)
        {
            Libro? libro = (from l in _bibliotecaContexto.Libro where l.id_libro == id select l).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }

            _bibliotecaContexto.Libro.Attach(libro);
            _bibliotecaContexto.Libro.Remove(libro);
            _bibliotecaContexto.SaveChanges();

            return Ok(libro);
        }

        

    }
}
