using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmICSCoreLib.Test
{
    public class TestKlasse
    {
        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "Vorname")]
        public string Vorname { get; set; }
        [JsonProperty(PropertyName = "Alter")]
        public int Age { get; set; }
        [JsonProperty(PropertyName = "Adresse")]
        public Adresse[] Adresse { get; set; }
    }

    public class Adresse
    {
        [JsonProperty(PropertyName = "Street")]
        public string Street { get; set; }
        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }
        [JsonProperty(PropertyName = "Zip")]
        public int Zip { get; set; }
    }
}
