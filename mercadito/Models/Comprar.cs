using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mercadito.Models
{
    internal class Comprar
    {


        public class Compra
        {
            public string message { get; set; }

            public Productos_Comprados[] productos_comprados { get; set; }
            public Productos_No_Comprados[] productos_no_comprados { get; set; }
        }


        public class Productos_Comprados
        {
            public int ProductoId { get; set; }
            public int Cantidad { get; set; }
          
        }

        public class Productos_No_Comprados
        {
            public int ProductoId { get; set; }
            public int Cantidad { get; set; }

        }


    }
}
