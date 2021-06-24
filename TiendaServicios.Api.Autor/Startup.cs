using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.Autor.Aplicacion;
using TiendaServicios.Api.Autor.ManejadorRabbit;
using TiendaServicios.Api.Autor.Persistencia;
using TiendaServicios.Mensajeria.Email.SendGridLibreria.Implement;
using TiendaServicios.Mensajeria.Email.SendGridLibreria.Interface;
using TiendaServicios.RabbitMQ.Bus.BusRabbit;
using TiendaServicios.RabbitMQ.Bus.EventoQueue;
using TiendaServicios.RabbitMQ.Bus.Implement;

namespace TiendaServicios.Api.Autor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /* Inyectando IRabbitEventBus e implementando el dependency injection */
            services.AddSingleton<IRabbitEventBus, RabbitEventBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new RabbitEventBus(sp.GetService < IMediator>(), scopeFactory);
            });
            /* Inyectnando los services de IsendGrid */
            services.AddSingleton<ISendGridEnviar, SendGridEnviar>();

            services.AddTransient<EmailEventoManejador>();

            /* Registrando el las clases del evento del RabbitMQ
             * Implementando IEventoManejador */

            services.AddTransient<IEventoManejador<EmailEventoQueue>, EmailEventoManejador>();
            services.AddDbContext<ContextoAutor>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("ConexionDatabase"));
            });
            /* Inicializar el FluentValidation, dentro de la clase Nuevo. */
            services.AddControllers().AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Nuevo>()); ;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TiendaServicios.Api.Autor", Version = "v1" });
            });
            /* Inyectando el mediatR */
            services.AddMediatR(typeof(Nuevo.Manejador).Assembly);
            /* Inyectando el AutoMapper */

            services.AddAutoMapper(typeof(Consulta.Manejador));
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TiendaServicios.Api.Autor v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            /* Registrar el eventobus  */
            var eventBus = app.ApplicationServices.GetRequiredService<IRabbitEventBus>();
            /* Suscribirlo al EmailEventoQueue, el cual es el evento con el mensaje */

            /* Objeto que fluye en el tubo de RabbitMQ con el mensaje | Clase que va manejar ese evento (escucha)  */
            eventBus.Subscribe<EmailEventoQueue, EmailEventoManejador>();
        }
    }
}
