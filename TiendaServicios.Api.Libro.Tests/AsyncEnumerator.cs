using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaServicios.Api.Libro.Tests
{
    /* Clase que evalua el arreglo que devuelve el entityFramework */
    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        /* Variable Globlal */
        private readonly IEnumerator<T> enumerator;

        public T Current => enumerator.Current;
        public AsyncEnumerator(IEnumerator<T> enumerator) => this.enumerator = enumerator ?? throw new ArgumentNullException();
        /* Eliminar el metodo */
        public async ValueTask DisposeAsync()
        {
            /* Cuando se complete la tarea */
            await Task.CompletedTask;
        }

        /*  */
        public async ValueTask<bool> MoveNextAsync()
        {
            /* Para que tambien devuelva los siguientes valores dentro del arreglo */
            return await Task.FromResult(enumerator.MoveNext());
        }
    }
}
