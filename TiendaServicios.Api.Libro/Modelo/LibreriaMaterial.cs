﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaServicios.Api.Libro.Modelo
{
    public class LibreriaMaterial
    {
        public Guid? LibreriaMaterialId { get; set; }
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        /* Esta data esta en otro Microservicio */
        public Guid? AutorLibro { get; set; }
    }
}
