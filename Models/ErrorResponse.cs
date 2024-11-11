using System.Collections.Generic;

namespace proyectoFinalMoviles.Models
{
    public class ErrorResponse
    {
        public List<string> message { get; set; }
        public int status { get; set; }
    }
}
