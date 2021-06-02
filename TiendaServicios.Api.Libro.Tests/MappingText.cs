using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Aplicacion;
using TiendaServicios.Api.Libro.Modelo;

namespace TiendaServicios.Api.Libro.Tests
{
    /* Mapear en DTO */
    public class MappingText : Profile
    {
        public MappingText()
        {
            CreateMap<LibreriaMaterial, LibroMaterialDto>();
        }
    }
}
