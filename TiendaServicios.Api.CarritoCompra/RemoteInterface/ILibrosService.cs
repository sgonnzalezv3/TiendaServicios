using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.RemoteModel;

namespace TiendaServicios.Api.CarritoCompra.RemoteInterface
{
    /* Operacion de Consulta */
    /* Obtener acceso al endpoint de libros */
    public interface ILibrosService
    {
        /* Tupla que tiene como contenido
         * Resultado : True o False dependiendo si la solicitud fue exitosa
         * Libro : tipo de dato el cual será utilizado para la deserialzación del json
         * ErrorMessage : Si la solicitud no fue exitosa, retorna un mensaje
         */
        Task<(bool resultado, LibroRemote Libro, string ErrorMessage)> GetLibro(Guid LibroId);
    }
}
