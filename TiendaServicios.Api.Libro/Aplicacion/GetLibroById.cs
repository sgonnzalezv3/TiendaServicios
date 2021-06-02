using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class GetLibroById
    {
        public class Ejecuta : IRequest<LibroMaterialDto>
        {
            public Guid? Id { get; set; }
        }
        public class Manejador : IRequestHandler<Ejecuta, LibroMaterialDto>
        {
            private readonly ContextoLibreria _context;
            private readonly IMapper _mapper;
            public Manejador(ContextoLibreria context,IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<LibroMaterialDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libro = await _context.LibreriaMaterial.Where(x => x.LibreriaMaterialId == request.Id).FirstOrDefaultAsync();
                if (libro == null)
                    throw new Exception("No existe el libro con el Id ingresado");
                var libroDto = _mapper.Map<LibreriaMaterial, LibroMaterialDto>(libro);
                return libroDto;
            }
        }
    }
}
