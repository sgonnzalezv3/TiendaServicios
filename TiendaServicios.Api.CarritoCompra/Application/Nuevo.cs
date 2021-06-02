using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Model;
using TiendaServicios.Api.CarritoCompra.Persistenci;

namespace TiendaServicios.Api.CarritoCompra.Application
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public DateTime FechaCreacionSesion { get; set; }
            public List<string> ProductoLista { get; set; }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CarritoContext _context;
            public Manejador(CarritoContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritoSesion = new CarritoSesion
                {
                    FechaCreacion = request.FechaCreacionSesion
                };
                _context.CarritoSesion.Add(carritoSesion);
                var value = await _context.SaveChangesAsync();
                if (value == 0)
                    throw new Exception("No se ha podido subir el carrito de compra");
                /* obtener Id autogenerado */
                int id = carritoSesion.CarritoSesionId;
                /* Agregar los detalles de los productos */
                foreach (var obj in request.ProductoLista)
                {
                    var detalleSesion = new CarritoSesionDetalle
                    {
                        FechaCreacion = DateTime.Now,
                        CarritoSesionId = id,
                        ProductoSeleccionado = obj
                    };
                    _context.CarritoSesionDetalle.Add(detalleSesion);
                }
                value = await _context.SaveChangesAsync();
                if (value > 0)
                    return Unit.Value;
                throw new Exception("No se ha podido insertar el detalle del carrito de compras");
            }
        }
    }
}
