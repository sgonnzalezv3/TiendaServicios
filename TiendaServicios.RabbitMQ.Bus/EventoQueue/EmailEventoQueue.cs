using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaServicios.RabbitMQ.Bus.Eventos;

namespace TiendaServicios.RabbitMQ.Bus.EventoQueue
{
    /* 
     Clase va generar objetos que van a fluir dentro del rabbitMQ Event bus
    - Crear un nuevo objeto y publicarlo dentro del rabbitmq event bus
     */
    public class EmailEventoQueue : Evento
    {
        public string Destinatario { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }

        public EmailEventoQueue(string destinatario, string titulo, string contenido)
        {
            Destinatario = destinatario;
            Titulo = titulo;
            Contenido = contenido;
        }
    }
}
