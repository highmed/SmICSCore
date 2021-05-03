using Newtonsoft.Json;

namespace SmICSCoreLib.StatistikDataModels
{
    public class DailyReport
    {
        [JsonProperty(PropertyName = "Bericht")]
        public Bericht Bericht { get; set; }
    }

    public class Bericht
    {
        [JsonProperty(PropertyName = "Stand")]
        public string Stand { get; set; }

        [JsonProperty(PropertyName = "StandAktuell")]
        public bool StandAktuell { get; set; }

        [JsonProperty(PropertyName = "Fallzahl")]
        public string Fallzahl { get; set; }

        [JsonProperty(PropertyName = "FallzahlVortag")]
        public string FallzahlVortag { get; set; }

        [JsonProperty(PropertyName = "Todesfaelle")]
        public string Todesfaelle { get; set; }

        [JsonProperty(PropertyName = "TodesfaelleVortag")]
        public string TodesfaelleVortag { get; set; }

        [JsonProperty(PropertyName = "RWert7Tage")]
        public string RWert7Tage { get; set; }

        [JsonProperty(PropertyName = "RWert7TageVortag")]
        public string RWert7TageVortag { get; set; }

        [JsonProperty(PropertyName = "Inzidenz7Tage")]
        public string Inzidenz7Tage { get; set; }

        [JsonProperty(PropertyName = "Inzidenz7TageVortag")]
        public string Inzidenz7TageVortag { get; set; }

        [JsonProperty(PropertyName = "GesamtImpfung")]
        public string GesamtImpfung { get; set; }

        [JsonProperty(PropertyName = "ImpfStatus")]
        public bool ImpfStatus { get; set; }

        [JsonProperty(PropertyName = "ErstImpfung")]
        public string ErstImpfung { get; set; }

        [JsonProperty(PropertyName = "ZweitImpfung")]
        public string ZweitImpfung { get; set; }

        [JsonProperty(PropertyName = "Bundesland")]
        public Bundesland[] Bundesland { get; set; }

        [JsonProperty(PropertyName = "BlStandAktuell")]
        public bool BlStandAktuell { get; set; }
    }

    public class Bundesland
    {
        [JsonProperty(PropertyName = "BlAttribute")]
        public BlAttribute BlAttribute { get; set; }

        [JsonProperty(PropertyName = "Landkreise")]
        public Landkreis[] Landkreise { get; set; }
    }

    public class Landkreis
    {
        [JsonProperty(PropertyName = "Stadt")]
        public string Stadt { get; set; }

        [JsonProperty(PropertyName = "LandkreisName")]
        public string LandkreisName { get; set; }

        [JsonProperty(PropertyName = "FallzahlGesamt")]
        public string FallzahlGesamt { get; set; }

        [JsonProperty(PropertyName = "Faelle7Lk")]
        public string Faelle7Lk { get; set; }

        [JsonProperty(PropertyName = "FaellePro100000Ew")]
        public string FaellePro100000Ew { get; set; }

        [JsonProperty(PropertyName = "Inzidenz7Tage")]
        public string Inzidenz7Tage { get; set; }

        [JsonProperty(PropertyName = "Todesfaelle")]
        public string Todesfaelle { get; set; }

        [JsonProperty(PropertyName = "Todesfaelle7Lk")]
        public string Todesfaelle7Lk { get; set; }
    }

    public class BlAttribute
    {
        [JsonProperty(PropertyName = "Bundesland")]
        public string Bundesland { get; set; }

        [JsonProperty(PropertyName = "FallzahlGesamt")]
        public string FallzahlGesamt { get; set; }

        [JsonProperty(PropertyName = "Faelle7BL")]
        public string Faelle7BL { get; set; }

        [JsonProperty(PropertyName = "FaellePro100000Ew")]
        public string FaellePro100000Ew { get; set; }

        [JsonProperty(PropertyName = "Todesfaelle")]
        public string Todesfaelle { get; set; }

        [JsonProperty(PropertyName = "Todesfaelle7BL")]
        public string Todesfaelle7BL { get; set; }

        [JsonProperty(PropertyName = "Inzidenz7Tage")]
        public string Inzidenz7Tage { get; set; }

        [JsonProperty(PropertyName = "Farbe")]
        public string Farbe { get; set; }
    }
}
