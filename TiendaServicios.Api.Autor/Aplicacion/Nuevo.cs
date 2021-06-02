using FluentValidation;
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
    public class Nuevo
    {
        /* Esta clase se comunica con el controller y recibe los parámetros */
        public class Ejecuta : IRequest
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public DateTime? FechaNacimiento { get; set; }

        }
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellido).NotEmpty();
            }
        } 
        /* Clase encargada de manejar la lógica */
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly ContextoAutor _contexto;
            public Manejador(ContextoAutor contexto)
            {
                _contexto = contexto;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var existe = await _contexto.AutorLibro.Where(x => x.Nombre == request.Nombre).FirstOrDefaultAsync();
                if (existe != null)
                    throw new Exception("Ya existe el autor con ese nombre");
                AutorLibro autor = new AutorLibro()
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    FechaNacimiento = request.FechaNacimiento,
                    AutorLibroGuid = Convert.ToString(Guid.NewGuid())
                };
                _contexto.AutorLibro.Add(autor);
                var result = await _contexto.SaveChangesAsync();
                if (result > 0)
                    return Unit.Value;
                throw new Exception("No se ha podido realizar el proceso!");
            }
        }
    }
}
