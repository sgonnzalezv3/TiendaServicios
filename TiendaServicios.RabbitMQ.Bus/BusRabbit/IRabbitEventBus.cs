using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaServicios.RabbitMQ.Bus.Comandos;
using TiendaServicios.RabbitMQ.Bus.Eventos;

namespace TiendaServicios.RabbitMQ.Bus.BusRabbit
{
    /* 
     * Definiendo los principales métodos del tubo(rabbitmq) para poder ingresar los eventos y como suscribirse a ellos de manera genérica.
     * */
    public interface IRabbitEventBus
    {
        Task EnviarComando<T>(T comando) where T : Comando;

        void Publish<T>(T @evento) where T : Evento;
        void Subscribe<T, TH>() where T : Evento
                               where TH : IEventoManejador<T>;

    }
}
