using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.General
{
    public class CSV
    {
        [JsonProperty(PropertyName = "AverageNumberOfStays")]
        public int AverageNumberOfStays { get; set; }
        [JsonProperty(PropertyName = "AverageNumberOfNosCases")]
        public int AverageNumberOfNosCases { get; set; }
        [JsonProperty(PropertyName = "AverageNumberOfContacts")]
        public int AverageNumberOfContacts { get; set; }
        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }
    }
}
