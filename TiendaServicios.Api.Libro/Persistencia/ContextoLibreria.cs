using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;

namespace TiendaServicios.Api.Libro.Persistencia
{
    public class ContextoLibreria : DbContext
    {
        /* Ctor por defecto */
        public ContextoLibreria()
        {

        }
        public ContextoLibreria(DbContextOptions<ContextoLibreria> options): base(options) { }
        /* Virtual permite sobreescribir a futuro.  */
        public virtual DbSet<LibreriaMaterial> LibreriaMaterial { get; set; }
    }
}
