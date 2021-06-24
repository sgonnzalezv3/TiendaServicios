using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;
using TiendaServicios.RabbitMQ.Bus.BusRabbit;
using TiendaServicios.RabbitMQ.Bus.EventoQueue;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class Nuevo 
    {
        public class Ejecuta : IRequest
        {
            public string Titulo { get; set; }
            public DateTime? FechaPublicacion { get; set; }
            public Guid? AutorLibro { get; set; }
        }
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
                RuleFor(x => x.AutorLibro).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            /* Inyectar el IRabbitEventBus para que sea posible enviar mensajes al bus tubo */
            private readonly ContextoLibreria _context;
            private readonly IRabbitEventBus _eventBus;
            public Manejador(ContextoLibreria context, IRabbitEventBus eventBus)
            {
                _context = context;
                _eventBus = eventBus;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libro = new LibreriaMaterial
                {
                    Titulo = request.Titulo,
                    FechaPublicacion = request.FechaPublicacion,
                    AutorLibro = request.AutorLibro,
                };
                _context.LibreriaMaterial.Add(libro);
                var result = await _context.SaveChangesAsync();

                _eventBus.Publish(new EmailEventoQueue("sgonzalezv3@ucentral.edu.co", request.Titulo, "Este contenido es un ejemplo"));

                if (result > 0)
                    return Unit.Value;

                throw new Exception("No se ha podido guardar el libro");

            }
        }
    }
}
