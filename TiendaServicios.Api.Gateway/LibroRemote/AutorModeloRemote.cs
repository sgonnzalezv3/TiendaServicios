using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaServicios.Api.Gateway.LibroRemote
{
    /* Maqueta para el mapeo de la data que vendra de la ms de autor */
    public class AutorModeloRemote
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        /* Valor universal para seguimiento desde otros microservicios */
        public string AutorLibroGuid { get; set; }
    }
}
