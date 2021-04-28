using Newtonsoft.Json;

namespace SmICSCoreLib.StatistikDataModels
{
    public class DailyReport
    {
        [JsonProperty(PropertyName = "bericht")]
        public Bericht Bericht { get; set; }
    }

    public class Bericht
    {
        [JsonProperty(PropertyName = "stand")]
        public string Stand { get; set; }

        [JsonProperty(PropertyName = "standAktuell")]
        public bool StandAktuell { get; set; }

        [JsonProperty(PropertyName = "fallzahl")]
        public string Fallzahl { get; set; }

        [JsonProperty(PropertyName = "fallzahlVortag")]
        public string FallzahlVortag { get; set; }

        [JsonProperty(PropertyName = "todesfaelle")]
        public string Todesfaelle { get; set; }

        [JsonProperty(PropertyName = "todesfaelleVortag")]
        public string TodesfaelleVortag { get; set; }

        [JsonProperty(PropertyName = "rWert7Tage")]
        public string RWert7Tage { get; set; }

        [JsonProperty(PropertyName = "rWert7TageVortag")]
        public string RWert7TageVortag { get; set; }

        [JsonProperty(PropertyName = "inzidenz7Tage")]
        public string Inzidenz7Tage { get; set; }

        [JsonProperty(PropertyName = "inzidenz7TageVortag")]
        public string Inzidenz7TageVortag { get; set; }

        [JsonProperty(PropertyName = "gesamtImpfung")]
        public string GesamtImpfung { get; set; }

        [JsonProperty(PropertyName = "ImpfStatus")]
        public bool ImpfStatus { get; set; }

        [JsonProperty(PropertyName = "erstImpfung")]
        public string ErstImpfung { get; set; }

        [JsonProperty(PropertyName = "zweitImpfung")]
        public string ZweitImpfung { get; set; }

        [JsonProperty(PropertyName = "bundesland")]
        public Bundesland[] Bundesland { get; set; }

        [JsonProperty(PropertyName = "blStandAktuell")]
        public bool BlStandAktuell { get; set; }
    }

    public class Bundesland
    {
        [JsonProperty(PropertyName = "blAttribute")]
        public BlAttribute BlAttribute { get; set; }

        [JsonProperty(PropertyName = "landkreis")]
        public Landkreis[] Landkreis { get; set; }
    }

    public class Landkreis
    {
        [JsonProperty(PropertyName = "stadt")]
        public string Stadt { get; set; }

        [JsonProperty(PropertyName = "LandkreisName")]
        public string LandkreisName { get; set; }

        [JsonProperty(PropertyName = "fallzahlGesamt")]
        public int FallzahlGesamt { get; set; }

        [JsonProperty(PropertyName = "faelle7Lk")]
        public float Faelle7Lk { get; set; }

        [JsonProperty(PropertyName = "faellePro100000Ew")]
        public float FaellePro100000Ew { get; set; }

        [JsonProperty(PropertyName = "inzidenz7Tage")]
        public float Inzidenz7Tage { get; set; }

        [JsonProperty(PropertyName = "todesfaelle")]
        public int Todesfaelle { get; set; }

        [JsonProperty(PropertyName = "todesfaelle7Lk")]
        public int Todesfaelle7Lk { get; set; }
    }

    public class BlAttribute
    {
        [JsonProperty(PropertyName = "bundesland")]
        public string Bundesland { get; set; }

        [JsonProperty(PropertyName = "fallzahlGesamt")]
        public int FallzahlGesamt { get; set; }

        [JsonProperty(PropertyName = "faelle7BL")]
        public float Faelle7BL { get; set; }

        [JsonProperty(PropertyName = "faellePro100000Ew")]
        public float FaellePro100000Ew { get; set; }

        [JsonProperty(PropertyName = "todesfaelle")]
        public int Todesfaelle { get; set; }

        [JsonProperty(PropertyName = "todesfaelle7BL")]
        public int Todesfaelle7BL { get; set; }

        [JsonProperty(PropertyName = "inzidenz7Tage")]
        public string Inzidenz7Tage { get; set; }

        [JsonProperty(PropertyName = "farbe")]
        public string Farbe { get; set; }
    }
}
