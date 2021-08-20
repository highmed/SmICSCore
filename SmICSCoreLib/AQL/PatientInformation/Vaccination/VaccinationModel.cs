using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.PatientInformation.Vaccination
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
        public string Dosierungsreihenfolge { get; set; }
        [JsonProperty(PropertyName = "Dosiermenge")]
        public string Dosiermenge { get; set; }
        [JsonProperty(PropertyName = "ImpfungGegen")]
        public string ImpfungGegen { get; set; }
        [JsonProperty(PropertyName = "Aussage wegen Abwesenheit")]
        public string Abwesendheit { get; set; }

    }
}
