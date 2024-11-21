using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proyectoFinalMoviles.Models
{
    public class Feature
    {
        public string model { get; set; }
        public string pk { get; set; }
        public FeatureFields fields { get; set; }
    }

    public class FeatureFields
    {
        public FeatureProperties properties { get; set; }
        public string layer_name { get; set; }
    }

    public class FeatureProperties
    {
        public string description { get; set; }
        public string feature_type { get; set; }
    }

}
