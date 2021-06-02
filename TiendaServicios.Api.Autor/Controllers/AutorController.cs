using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.Autor.Aplicacion;
using TiendaServicios.Api.Autor.Modelo;

namespace TiendaServicios.Api.Autor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AutorController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            /* Envia la data a la clase nuevo de Aplicacion */
            return await _mediator.Send(data);
        }
        [HttpGet]
        public async Task<ActionResult<List<AutorLibroDto>>> GetAutores()
        {
            /* Ejecuta la lógica de la clase Consulta y devuelve la data (lista) */
            return await _mediator.Send(new Consulta.ListaAutor());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AutorLibroDto>> GetAutorById(string id)
        {
            return await _mediator.Send(new ConsultaById.autor { AutorGuid = id });
        }
    }
}
