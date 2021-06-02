using AutoMapper;
using GenFu;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Aplicacion;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;
using Xunit;

namespace TiendaServicios.Api.Libro.Tests
{
    public class LibrosServiceTest
    {

        private IEnumerable<LibreriaMaterial> ObtenerDataPrueba()
        {
            /* indicar CLASE QUE VA DAR estrucutra a la data fake */
            A.Configure<LibreriaMaterial>()
                /* AsArticleTitle: generar un nombre al titulo */
                .Fill(x => x.Titulo).AsArticleTitle()
                .Fill(x => x.LibreriaMaterialId, () => { return Guid.NewGuid(); });
            /* Crear lista de objetos aleatorios */

            var lista = A.ListOf<LibreriaMaterial>(30);
            /* se va querer realizar una funcionalidad para obtener un libro dado su id
             * para ello encesitamos saber un id:
             * entonces el dato de la posicion 0 le asignamos un valor conocido
             */
            lista[0].LibreriaMaterialId = Guid.Empty;
            return lista;
        }
        /* Procedimiento para agregar la data de obtenerDatPrueba a mockContexto */
        private Mock<ContextoLibreria> CrearContexto()
        {
            /* Obtener data de prueba */
            var dataPrueba = ObtenerDataPrueba().AsQueryable();
            /* nuevo DbSet */
            var dbSet = new Mock<DbSet<LibreriaMaterial>>();
            /* Libreriamaterial debe ser entidad, pero la clase de ef debe tener  los siguientes : */
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Provider).Returns(dataPrueba.Provider);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Expression).Returns(dataPrueba.Expression);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.ElementType).Returns(dataPrueba.ElementType);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.GetEnumerator()).Returns(dataPrueba.GetEnumerator());
            /* Implementar clases */
            dbSet.As<IAsyncEnumerable<LibreriaMaterial>>().Setup(x => x.GetAsyncEnumerator(new System.Threading.CancellationToken()))
                .Returns(new AsyncEnumerator<LibreriaMaterial>(dataPrueba.GetEnumerator()));
            /*agregando provider para filtros a la entidad LibreriaMaterial*/
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Provider).Returns(new AsyncQueryProvider<LibreriaMaterial>(dataPrueba.Provider));
            /* Instancia de contexto */
            var contexto = new Mock<ContextoLibreria>();
            contexto.Setup(x => x.LibreriaMaterial).Returns(dbSet.Object);
            return contexto;
        }

        [Fact]
        public async void GetLibroById()
        {
            /* Contexot */
            var mokContexto = CrearContexto();

            /* Maper de ef a clase Dto  */
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingText());
            });
            /* Creaci►2n del Mapper */
            var mapper = mapConfig.CreateMapper();

            /* Crear el objeto que va almacenar el parametro de libroId y a su vez insertarlo dentro del metodo handler, el cual se encarga de hacer la
            transacción para hacer la búsqueda del libro.
            */
            var request = new GetLibroById.Ejecuta();
            /* buscando el libro con el Id 0000 */
            request.Id = Guid.Empty;

            /* Instanciar al objeto manejador */
            var manejador = new GetLibroById.Manejador(mokContexto.Object, mapper);

            /* llamar a su parameto */
            var libro = await manejador.Handle(request, new System.Threading.CancellationToken());
            /* Si devuelve un libro, es true */
            Assert.NotNull(libro);
            /* tiene que tener el Guid 0000 */
            Assert.True(libro.LibreriaMaterialId == Guid.Empty);

        }
        [Fact]
        public async  void GetLibros()
        {
            /*Debug*/
          //  System.Diagnostics.Debugger.Launch();

            /* que metodo dentro del MS libro se está encargando de realizar la consulta de libros
             * de la BD*/

            /* Emular instancia de EF Core */
            /* Para emular objetos de en Test se usan MOCKS*/

            /* mock que simula el contextoLibreria (ConexionEF) */
            var mockContexto = CrearContexto();

            /* Emular IMapper */
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingText());
            });

            /* Objeto mapper */
            var mapper = mapConfig.CreateMapper();
            /* Instanciar clase Manejador y pasarle como parametros los MOCKS */
            Consulta.Manejador manejador = new Consulta.Manejador(mockContexto.Object, mapper);
            Consulta.Ejecuta request = new Consulta.Ejecuta();

            var lista = await manejador.Handle(request, new System.Threading.CancellationToken());

            /* Si la lista tiene algú valor (el test pasa)*/
            Assert.True(lista.Any());

        }
        [Fact]
        public async void GuardarLibro()
        {
            System.Diagnostics.Debugger.Launch();
            /* Instancia de base de datos en Memoria */
            var options = new DbContextOptionsBuilder<ContextoLibreria>()
                .UseInMemoryDatabase(databaseName: "BaseDatosLibros")
                .Options;
            /* Instanciar Ejecuta */
            var contexto = new ContextoLibreria(options);
            var request = new Nuevo.Ejecuta();
            request.Titulo = "Libro de Millos";
            request.AutorLibro = Guid.Empty;
            request.FechaPublicacion = DateTime.Now;
            var manejador = new Nuevo.Manejador(contexto);
            var libro = await manejador.Handle(request, new System.Threading.CancellationToken());
            
            Assert.True(libro != null);


        }
    }
}
