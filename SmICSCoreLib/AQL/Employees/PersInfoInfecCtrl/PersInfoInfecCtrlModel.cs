using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace SmICSCoreLib.AQL.Employees.PersInfoInfecCtrl
{
    public class PersInfoInfecCtrlModel
    {
        [JsonProperty(PropertyName = "berichtID")]
        [Required]
        public string berichtID { get; set; }
        [JsonProperty(PropertyName = "dokumentations_id")]
        [Required]
        public DateTime dokumentations_id { get; set; }
        [JsonProperty(PropertyName = "symp_vorhanden")]
        [Required]
        public string symp_vorhanden { get; set; }
        [JsonProperty(PropertyName = "symp_auftreten")]
        [Required]
        public string symp_auftreten { get; set; }
        [JsonProperty(PropertyName = "bez_symp")]
        [Required]
        public string bez_symp { get; set; }
        [JsonProperty(PropertyName = "allg_kommentar")]
        public string allg_kommentar { get; set; }
        [JsonProperty(PropertyName = "erreg_nachweis")]
        public bool erreg_nachweis { get; set; } = false;
        [JsonProperty(PropertyName = "erreg_name")]
        [Required]
        public string erreg_name { get; set; }
        [JsonProperty(PropertyName = "zeitpunkt_kennzeichnung")]
        [Required]
        public string zeitpunkt_kennzeichnung { get; set; }
        [JsonProperty(PropertyName = "erreg_Nachweis_klinik")]
        public bool erreg_Nachweis_klinik { get; set; } = false;
        [JsonProperty(PropertyName = "zuletzt_aktuell")]
        [Required]
        public string zuletzt_aktuell { get; set; }
        [JsonProperty(PropertyName = "freigetsellt")]
        public bool freigetsellt { get; set; } = false;
        [JsonProperty(PropertyName = "grund")]
        [Required]
        public string grund { get; set; }
        [JsonProperty(PropertyName = "beschreibung")]
        [Required]
        public string beschreibung { get; set; }
        [JsonProperty(PropertyName = "start_freistellung")]
        [Required]
        public Nullable<DateTime> start_freistellung { get; set; } = null;
        [JsonProperty(PropertyName = "wiederaufnahme")]
        [Required]
        public Nullable<DateTime> wiederaufnahme { get; set; } = null;
        [JsonProperty(PropertyName = "kommentar_freistellung")]
        public string kommentar_freistellung { get; set; }
        [JsonProperty(PropertyName = "meldung")]
        public bool meldung { get; set; } = false;
        [JsonProperty(PropertyName = "ereignis")]
        [Required]
        public string ereignis { get; set; }
        [JsonProperty(PropertyName = "beschreibung_ereignis")]
        [Required]
        public string beschreibung_ereignis { get; set; }
        [JsonProperty(PropertyName = "datum_meldung")]
        [Required]
        public string datum_meldung { get; set; }
        [JsonProperty(PropertyName = "grund_meldung")]
        [Required]
        public string grund_meldung { get; set; }
        [JsonProperty(PropertyName = "meldung_kommentar")]
        public string meldung_kommentar { get; set; }
    }
}
