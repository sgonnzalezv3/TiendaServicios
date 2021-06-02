using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Persistenci;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;

namespace TiendaServicios.Api.CarritoCompra.Application
{
    public class Consulta
    {
        public class Ejecuta : IRequest<CarritoDto>
        {
            public int CarritoSesionId { get; set; }
        }
        public class Manejador : IRequestHandler<Ejecuta, CarritoDto>
        {
            private readonly CarritoContext _context;
            private readonly ILibrosService _libroService;
            public Manejador( CarritoContext context, ILibrosService libroService)
            {
                _context = context;
                _libroService = libroService;
            }
            public async Task<CarritoDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                /* Obtener el carrito almacenado en MySQL */
                var carritoSesion = await _context.CarritoSesion.FirstOrDefaultAsync(x => x.CarritoSesionId == request.CarritoSesionId);
                /* Lista de productos que tiene el carrito sesion detalle */
                /* Solo los ID's de los productos, para conocer el detalle de cada producto, se necesita invocar el Endpoint _libroService  */
                var carritoSesionDetalle = await _context.CarritoSesionDetalle.Where(x=> x.CarritoSesionId == request.CarritoSesionId).ToListAsync();
                /* Por cada libro que exista en la bd MYsql, invoca la MS y devuelve la data del libro */
                var listaCarritoDto = new List<CarritoDetalleDto>();
                /* Por cada libro que exista en la MS */
                foreach (var libro in carritoSesionDetalle)
                {
                    /*  */
                    var response = await _libroService.GetLibro(new Guid(libro.ProductoSeleccionado));
                    /* dentro del response esta toda la data JSon */
                    if (response.resultado)
                    {
                        /* Data LibroRemote, necesitamos convertirla en tipo CarritoDetalleDto */
                        var objetoLibro = response.Libro;

                        var carritoDetalle = new CarritoDetalleDto
                        {
                            TituloLibro = objetoLibro.Titulo,
                            FechaPublicacion = objetoLibro.FechaPublicacion,
                            LibroId = objetoLibro.LibreriaMaterialId
                        };
                        listaCarritoDto.Add(carritoDetalle);
                    }
                }
                /* Se crea CarritoDto */
                var carritoSesionDto = new CarritoDto
                {
                    CarritoId = carritoSesion.CarritoSesionId,
                    FechaCreacionSesion = carritoSesion.FechaCreacion,
                    ListaProductos = listaCarritoDto
                };
                return carritoSesionDto;

            }
        }
    }
}
