using ApiPracticaBiblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;


namespace ApiPracticaBiblioteca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly bibliotecaContext _bibliotecaContexto;

        public AutorController(bibliotecaContext bibliotecaContext)
        {
            _bibliotecaContexto = bibliotecaContext;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Autor> listadoAutor = (from a in _bibliotecaContexto.Autor select a).ToList();

            if (listadoAutor.Count == 0)
            {
                return NotFound();
            }

            return Ok(listadoAutor);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var autor = (from a in _bibliotecaContexto.Autor
                         join l in _bibliotecaContexto.Libro
                         on a.id_autor equals l.id_autor
                         where l.id_libro == id
                         select new
                         {
                             a.id_autor,
                             a.nombre,
                             a.nacionalidad,
                             l.id_libro,
                             libro = l.titulo,
                         }).FirstOrDefault();

            if (autor == null)
            {
                return NotFound();
            }

            return Ok(autor);
        }

        //Crear
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarAutor([FromBody] Autor autor)
        {
            try
            {
                _bibliotecaContexto.Autor.Add(autor);
                _bibliotecaContexto.SaveChanges();
                return Ok(autor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Actualizar
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarAutor(int id, [FromBody] Autor autorModificar)
        {
            Autor? autorActual = (from a in _bibliotecaContexto.Autor where a.id_autor == id select a).FirstOrDefault();

            if (autorActual == null)
            {
                return NotFound();
            }

            autorActual.nombre = autorModificar.nombre;
            autorActual.nacionalidad = autorModificar.nacionalidad;

            _bibliotecaContexto.Entry(autorActual).State = EntityState.Modified;
            _bibliotecaContexto.SaveChanges();

            return Ok(autorModificar);
        }

        //Eliminar
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarAutor(int id)
        {
            Autor? autor = (from a in _bibliotecaContexto.Autor where a.id_autor == id select a).FirstOrDefault();

            if (autor == null)
            {
                return NotFound();
            }

            _bibliotecaContexto.Autor.Attach(autor);
            _bibliotecaContexto.Autor.Remove(autor);
            _bibliotecaContexto.SaveChanges();

            return Ok(autor);
        }
    }
}
