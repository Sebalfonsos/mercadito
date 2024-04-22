using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mercadito.Models
{
    internal class UserLogged
    {


        public class ApiResponse
        {
            public bool status { get; set; }
            public string message { get; set; }
            public UserData data { get; set; }
            public string token { get; set; }
        }

        public class UserData
        {
            public string names { get; set; }
        }
        
        

    }
}