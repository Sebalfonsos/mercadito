using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mercadito.Models
{
    internal class Productos
    {

        public class Producto
        {
            public Class1[] producto { get; set; }
        }

        public class Class1
        {
            public int id { get; set; }
            public string nombre { get; set; }
            public string descripcion { get; set; }
            public string imagen { get; set; }
            public int precio { get; set; }
            public int cantidad { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

    }
}
