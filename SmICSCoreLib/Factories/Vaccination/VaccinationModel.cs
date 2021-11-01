using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.Factories.Vaccination
{
    public class VaccinationModel
    {
        [JsonProperty(PropertyName ="PatientenID")]
        public string PatientenID { get; set; }
        [JsonProperty(PropertyName = "DokumentationsID")]
        public DateTime DokumentationsID { get; set; }
        [JsonProperty(PropertyName = "Impfstoff")]
        public string Impfstoff { get; set; }
        [JsonProperty(PropertyName = "Dosierungsreihenfolge")]
        public int Dosierungsreihenfolge { get; set; }
        [JsonProperty(PropertyName = "Dosiermenge")]
        public int Dosiermenge { get; set; }
        [JsonProperty(PropertyName = "ImpfungGegen")]
        public string ImpfungGegen { get; set; }
        [JsonProperty(PropertyName = "Abwesenheit")]
        public string Abwesendheit { get; set; }

    }
}
