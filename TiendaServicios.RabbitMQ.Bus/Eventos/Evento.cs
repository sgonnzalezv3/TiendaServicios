using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaServicios.RabbitMQ.Bus.Eventos
{
    public abstract class Evento
    {
        public DateTime Timestamp { get; protected set; }
        protected Evento()
        {
            Timestamp = DateTime.Now;
        }
    }
}
