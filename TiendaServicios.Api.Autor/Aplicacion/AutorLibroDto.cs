using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaServicios.Api.Autor.Aplicacion
{
    public class AutorLibroDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        /* Valor universal para seguimiento desde otros microservicios */
        public string AutorLibroGuid { get; set; }
    }
}
