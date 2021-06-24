using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaServicios.RabbitMQ.Bus.BusRabbit;
using TiendaServicios.RabbitMQ.Bus.Comandos;
using TiendaServicios.RabbitMQ.Bus.Eventos;

namespace TiendaServicios.RabbitMQ.Bus.Implement
{
    /* Corazon del proyecto
     ya que tiene toda la implementacion logica para publicar mensajes eventos y suscribir los eventos que ocurren en el bus.

     */
     
    public class RabbitEventBus : IRabbitEventBus
    {
        /* Inyectando objeto mediaTR
         * Con el fin de enviar y publicar eventos al bus
         * para ello se necesita una instancia
         * hacia las clases genéricas, para esto es MediatR
         */

        /* 
         RabbitMQ va manejar una lista de  manejadores y tipos
         */

        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _manejadores;
        private readonly List<Type> _eventoTipos;
        /* instancia dependency injection */

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitEventBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory)
        {
            _mediator = mediator;
            _manejadores = new Dictionary<string, List<Type>>();
            _eventoTipos = new List<Type>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task EnviarComando<T>(T comando) where T : Comando
        {
            return _mediator.Send(comando);
        }

        public void Publish<T>(T evento) where T : Evento
        {
            /* Conexion al servidor RabbitMQ        Pasandole el nombre del servidor donde esta alojado el rabbitMQ */
            var factory = new ConnectionFactory() { HostName = "rabbit-santi-web" };
            /* Abrir la conexion */
            using (var connection = factory.CreateConnection())
            /* Abrir el canal de comunicación */
            using (var channel = connection.CreateModel())
            {
                /* Nombre del evento a insertar al bus */
                var eventName = evento.GetType().Name;

                /*Indicar el nombre del Queue, el cual sera el nombre del evento */
                channel.QueueDeclare(eventName, false, false, false, null);
                /* ya se ha creado el queue, Ahora se agrega el mensaje dentro de ella */
                var message = JsonConvert.SerializeObject(evento);
                /* Cuerpo del mensaje */
                var body = Encoding.UTF8.GetBytes(message);
                /* publicando */
                /* pide : direccion,nombre del evento, null, y el cuerpo o body del mensaje */
                channel.BasicPublish("", eventName, null, body);
            }
        }

        /* El evento necesita un objeto manejador que permita disparar sus acciones */
        public void Subscribe<T, TH>()
            where T : Evento
            where TH : IEventoManejador<T>
        {
            /* Obtener el nombre del evento que se quiere obtener */
            var eventoNombre = typeof(T).Name;
            /* Tipo de manejador */
            var manejadorEventoTipo = typeof(TH);
            /* Agregar el evento a la lista de eventos */
            /* Validar si existe o no para evitar redundancias */
            if (!_eventoTipos.Contains(typeof(T)))
                /* Si no lo contiene, lo agrega */
                _eventoTipos.Add(typeof(T));
            /* Nuevo manejador de eventos */
            /* Validar si existe o no el evento */
            if (!_manejadores.ContainsKey(eventoNombre))
                /* Si no lo contiene, lo agrega */
                _manejadores.Add(eventoNombre, new List<Type>());

            /* Saber si el manejador ya ha registrado anteriormente en otro contexto al manejadorEventoTipo */
            /* de ser asi, se debe ejecutar un throw exception */
            if (_manejadores[eventoNombre].Any(x => x.GetType() == manejadorEventoTipo))
                throw new ArgumentException($"El manejador {manejadorEventoTipo.Name} fue registrado anteriormente por {eventoNombre}");

            _manejadores[eventoNombre].Add(manejadorEventoTipo);

            /* Abrir conexion para iniciar el consumo */
            var factory = new ConnectionFactory()
            {
                /* Nombre del server */
                HostName = "rabbit-santi-web",
                DispatchConsumersAsync = true
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            /* Declaracion para saber desde cual queue se quiere consumir los mensajes */
            channel.QueueDeclare(eventoNombre, false, false, false, null);
            /* Objeto consumer pasando el channel*/
            /* Esto va inicializar el queue */
            var consumer = new AsyncEventingBasicConsumer(channel);
            /* Trabajo con delegate encargado de leer los mensajes del queue */
            consumer.Received += Consumer_Delegate;
            /*                  nombre del queue  | reconocer los mensajes true | nombre del consumer creado */
            channel.BasicConsume(eventoNombre, true, consumer);


        }

        private async Task Consumer_Delegate(object sender, BasicDeliverEventArgs e)
        {
            /* capturar nombre del queue o evento */
            var nombreEvento = e.RoutingKey;
            /* Capturar el mensaje */
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            /* Try para lectura de string */
            try
            {
                /* Validar que el arreglo de manejadores contenga el nombre del queue que se quiere trabajar */
                if (_manejadores.ContainsKey(nombreEvento))
                {
                    /* Permitir inyecciones _serviceScopeFactory*/
                    /* la clase que contiene handle va poder buscar nuevos objetos para inyectar o procesar el mensaje */
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        /* Obtener todos las suscripciones en las que este el nombre evento */
                        var subscriptions = _manejadores[nombreEvento];
                        foreach (var sb in subscriptions)
                        {
                            /* por cada suscripcion crear un manejador individual */
                            var manejador = scope.ServiceProvider.GetService(sb); //Activator.CreateInstance(sb);
                            /* Si no se creo correctamente, quiere decir que es nulo, entonces que continue con el siguiente */
                            if (manejador == null) continue;

                            /* si no es nulo */
                            var tipoEvento = _eventoTipos.SingleOrDefault(x => x.Name == nombreEvento);
                            /* deserializar tipoEvento, pasandole el contenido del mensaje, y el tipo de evento */
                            var eventoDS = JsonConvert.DeserializeObject(message, tipoEvento);
                            /* Crear un tipo concreto pasandole la interface para castearlo*/
                            var concretoTipo = typeof(IEventoManejador<>).MakeGenericType(tipoEvento);
                            /* Crear metodo para leer mensajes de queue */
                            await (Task)concretoTipo.GetMethod("Handle").Invoke(manejador, new object[] { eventoDS });
                        }
                    }

                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}
