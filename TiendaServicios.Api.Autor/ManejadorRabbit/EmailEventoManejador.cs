using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Mensajeria.Email.SendGridLibreria.Interface;
using TiendaServicios.Mensajeria.Email.SendGridLibreria.Modelo;
using TiendaServicios.RabbitMQ.Bus.BusRabbit;
using TiendaServicios.RabbitMQ.Bus.EventoQueue;

namespace TiendaServicios.Api.Autor.ManejadorRabbit
{
    /* Cada vez que detecte que hay un evento EmailEventoQueue dentro del tubo, automaticamente dispara la clase y el método
     Consumiendo el mensaje e imprimiendolo dentro del logger.
     */
    public class EmailEventoManejador : IEventoManejador<EmailEventoQueue>
    {
        /* Inyectando para imprimir el valor del event */
        private readonly ILogger<EmailEventoManejador> _logger;
        /* Instancia de objeto que permite crear correos */

        private readonly ISendGridEnviar _sendGridEnviar;
        /* Inyectamos esto que nos permite traer data del appsettings.json */
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public EmailEventoManejador()
        {

        }
        
        public EmailEventoManejador( ILogger<EmailEventoManejador> logger, ISendGridEnviar sendGridEnviar, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _logger = logger;
            _sendGridEnviar = sendGridEnviar;
            _configuration = configuration;
        }
        

        /* @event entrega la data del mensaje, es el objeto mensaje */
        public async Task Handle(EmailEventoQueue @event)
        {
            _logger.LogInformation($"Valor que consumo desde el RabbitMQ {@event.Titulo}");
            /* Objeto SendGridData */
            var objData = new SendGridData();
            objData.Contenido = @event.Contenido;
            objData.EmailDestinatario = @event.Destinatario;
            objData.NombreDestinatario = @event.Destinatario;
            objData.Titulo = @event.Titulo;
            objData.SendGridAPIKey = _configuration["SendGrid:ApiKey"];

            
            var result = await _sendGridEnviar.EnviarEmail(objData);
            /* Si el resultado es exitoso*/
            if (result.resultado)
            {
                /* Complete la tarea y termine la construccion del mssage */
                await Task.CompletedTask;
                return;
            }
        }
    }
}
