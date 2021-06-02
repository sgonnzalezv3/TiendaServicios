using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;
using TiendaServicios.Api.CarritoCompra.RemoteModel;

namespace TiendaServicios.Api.CarritoCompra.RemoteService
{
    /* Clase encargada de implementar los metodos de la Interface ILibrosService */

    public class LibrosService : ILibrosService
    {
        private readonly IHttpClientFactory _httpClient;

        /* Indicar en la clase en el que va trabajar(LibrosService) */
        private readonly ILogger<LibrosService> _logger;

        public LibrosService(IHttpClientFactory httpClient, ILogger<LibrosService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<(bool resultado, LibroRemote Libro, string ErrorMessage)> GetLibro(Guid LibroId)
        {
            try
            {
                /* Se obtiene del servicio addhttpcliente en el startup */
                /* creande nuevo cliente, y tomando la url base */
                var cliente = _httpClient.CreateClient("Libros");
                /* Invocar endpoint de libro, pasandole el parametro del Id (Controller de libro) */
                /* Devuelve un response Json con la data */
                var response = await cliente.GetAsync($"api/LibroMaterial/{LibroId}");
                /* Analizar si fue exitoso */
                if (response.IsSuccessStatusCode)
                {
                    /* Despliega la data */
                    var contenido = await response.Content.ReadAsStringAsync();
                    /* Propiedad para no tener problemas con mayusculas o minusculas */
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    /* Match con el LibroRemote, que es el esquema al cual se va transformar*/
                    var resultado = JsonSerializer.Deserialize<LibroRemote>(contenido, options);
                    /* Retorna la tupla con la data */
                    return (true, resultado, null);
                }
                return (false, null, response.ReasonPhrase);
            }
            catch (Exception e)
            {
                _logger?.LogError(e.ToString());
                return (false, null, e.Message);
            }
        }
    }
}
