using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace proyectoFinalMoviles.Models
{
    public class Issue
    {
        public string id { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        public string evidence { get; set; }
        public string batch_id { get; set; }
        public string user_id { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime createdAt { get; set; }
    }

}
