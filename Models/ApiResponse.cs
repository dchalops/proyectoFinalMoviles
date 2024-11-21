using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proyectoFinalMoviles.Models
{
    public class ApiResponse<T>
    {
        public string message { get; set; }
        public T response { get; set; }
    }
}
