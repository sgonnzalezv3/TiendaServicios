using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using TiendaServicios.Mensajeria.Email.SendGridLibreria.Interface;
using TiendaServicios.Mensajeria.Email.SendGridLibreria.Modelo;

namespace TiendaServicios.Mensajeria.Email.SendGridLibreria.Implement
{
    public class SendGridEnviar : ISendGridEnviar
    {
        public async Task<(bool resultado, string errorMessage)> EnviarEmail(SendGridData data)
        {
            try
            {
                /* Instancia de SendGrid pasandole el ApiKey*/
                var sendGridCliente = new SendGridClient(data.SendGridAPIKey);
                /* Indicar el destinatario pasando Email | nombre del destinatario */
                var destinatario = new EmailAddress(data.EmailDestinatario, data.NombreDestinatario);
                /* Titulo */
                var tituloEmail = data.Titulo;
                /* Quien envia el mensaje */
                var sender = new EmailAddress("sgonzalezv3@ucentral.edu.co", "Santiago Gonzalez");
                /* Contenido del mensaje */
                var contenidoMensaje = data.Contenido;
                /* Objeto tipo SendGridMessage (creando el mensaje como tal)*/
                var objMensaje = MailHelper.CreateSingleEmail(sender, destinatario, tituloEmail, contenidoMensaje, contenidoMensaje);
                /* Enviar el mensaje */
                await sendGridCliente.SendEmailAsync(objMensaje);
                return (true, null);
            }
            catch (Exception e)
            {
                return (false,e.Message);
            }
        }
    }
}
