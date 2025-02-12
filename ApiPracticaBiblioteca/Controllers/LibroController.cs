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

        [HttpGet]
        [Route("Find/{filtro}")]
        public IActionResult FindByAnio(string filtro)
        {
            Libro? libro = _bibliotecaContexto.Libro
                .Where(l => l.anioPublicacion.ToString().Contains(filtro)) 
                .FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }

            return Ok(libro);
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
