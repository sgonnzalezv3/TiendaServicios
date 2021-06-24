using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Autor.Modelo;
using TiendaServicios.Api.Autor.Persistencia;

namespace TiendaServicios.Api.Autor.Aplicacion
{
    public class ConsultaById
    {
        public class autor : IRequest<AutorLibroDto>
        {
            public string AutorGuid { get; set; }
        }
        public class Manejador : IRequestHandler<autor, AutorLibroDto>
        {
            private readonly ContextoAutor _contexto;
            private readonly IMapper _mapper;
            public Manejador(ContextoAutor contexto, IMapper mapper)
            {
                _contexto = contexto;
                _mapper = mapper;
            }
            public async  Task<AutorLibroDto> Handle(autor request, CancellationToken cancellationToken)
            {
                var autor = await _contexto.AutorLibro.Where(x=> x.AutorLibroGuid == request.AutorGuid).FirstOrDefaultAsync();
                if (autor == null)
                    throw new Exception("No existe el autor con esa identificacion");
                var autorDto = _mapper.Map<AutorLibroDto>(autor);
                return autorDto; 
            }
        }
    }
}
