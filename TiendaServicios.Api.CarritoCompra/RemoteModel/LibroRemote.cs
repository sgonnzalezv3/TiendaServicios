using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaServicios.Api.CarritoCompra.RemoteModel
{
    /* Estructura con la que vamos a recibir la data */
    /* Misma estrucutra en la microservice de Libros  */
    public class LibroRemote
    {
        public Guid? LibreriaMaterialId { get; set; }
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        /* Esta data esta en otro Microservicio */
        public Guid? AutorLibro { get; set; }
    }
}
