using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Gateway.InterfaceRemote;
using TiendaServicios.Api.Gateway.LibroRemote;

namespace TiendaServicios.Api.Gateway.MessageHandler
{
    /*
        Http Message Handler
        Intercepta los mensajes que se envian entre un cliente y un backend server(ocelot)
        Cuando se envia un request, este pasa por el componente y se lo envia al ocelot.
        
    caso de vuelta:
        Cuando Ocelot quiera retornar un objeto response al cliente, antes de que llege al objetivo final
        sera procesado por el Message Handler
        En este punto, cuando se recibe la respuesta del libro en especifico, se realiza otro request para obtener el autor de ese libro
        y dar una respuesta compuesta al cliente final.
     */


    public class LibroHandler : DelegatingHandler
    {
        /* Inyectando el logger en el constructor */
        private readonly ILogger<LibroHandler> _logger;

        /* Consumir servicio de AutorRemote */
        private readonly IAutorRemote _autorRemote;

        public LibroHandler(ILogger<LibroHandler> logger, IAutorRemote autorRemote)
        {
            _logger = logger;
            _autorRemote = autorRemote;
        }
        /* Funcion encargada de obtener el tiempo de respuesta de la consulta.  */
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request , CancellationToken cancellationToken)
        {
            var tiempo = Stopwatch.StartNew();
            _logger.LogInformation("Inicia el request");
            var response = await base.SendAsync(request, cancellationToken);
            /* Verificar respuesta correcta */
            if (response.IsSuccessStatusCode)
            {
                /* Obteniendo la data */
                var contenido = await response.Content.ReadAsStringAsync();
                /* Deserializar la data */
                var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true};

                /* Mapearla en la maqueta de objeto creado (LibroModeloRemote) */
                /*1 parametro: contenido string que viene del servidor de la ms de libro
                  2 parametro: options (opciones)
                 */
                var resultado = JsonSerializer.Deserialize<LibroModeloRemote>(contenido, options);
                /* Consumir IAutorRemote */
                var responseAutor = await _autorRemote.GetAutor(resultado.AutorLibro ?? Guid.Empty);
                /* Si la respuesta es correcta */
                if (responseAutor.resultado)
                {
                    /* Obtener la data del libro */
                    var objetoAutor = responseAutor.autor;
                    resultado.AutorData = objetoAutor;
                    /* Convertir a objeto json */
                    var resultadoStr = JsonSerializer.Serialize(resultado);
                    /* Cambiar el contenido de response */
                    response.Content = new StringContent(resultadoStr, System.Text.Encoding.UTF8,"application/json");
                }
            }
            _logger.LogInformation($"Este proceso se ha realizado en {tiempo.ElapsedMilliseconds}ms");
            return response;
        }
    }
}
