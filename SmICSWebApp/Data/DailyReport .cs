using Newtonsoft.Json;

namespace SmICSWebApp.Data
{
    public class DailyReport
    {
        [JsonProperty(PropertyName = "bericht")]
        public Bericht bericht { get; set; }
    }

    public class Bericht
    {
        [JsonProperty(PropertyName = "stand")]
        public string stand { get; set; }

        [JsonProperty(PropertyName = "standAktuell")]
        public bool standAktuell { get; set; }

        [JsonProperty(PropertyName = "fallzahl")]
        public string fallzahl { get; set; } 
        
        [JsonProperty(PropertyName = "fallzahlVortag")]
        public string fallzahlVortag { get; set; }   

        [JsonProperty(PropertyName = "todesfaelle")]
        public string todesfaelle { get; set; } 
        
        [JsonProperty(PropertyName = "todesfaelleVortag")]
        public string todesfaelleVortag { get; set; }

        [JsonProperty(PropertyName = "rWert7Tage")]
        public string rWert7Tage { get; set; } 
        
        [JsonProperty(PropertyName = "rWert7TageVortag")]
        public string rWert7TageVortag { get; set; }
        
        [JsonProperty(PropertyName = "inzidenz7Tage")]
        public string inzidenz7Tage { get; set; }  
        
        [JsonProperty(PropertyName = "inzidenz7TageVortag")]
        public string inzidenz7TageVortag { get; set; }

        [JsonProperty(PropertyName = "bundesland")]
        public Bundesland[] bundesland { get; set; }

        [JsonProperty(PropertyName = "blStandAktuell")]
        public bool blStandAktuell { get; set; }
    }

    public class Bundesland
    {
        [JsonProperty(PropertyName = "blAttribute")]
        public BlAttribute blAttribute { get; set; }

        [JsonProperty(PropertyName = "landkreis")]
        public Landkreis[] landkreis { get; set; }
    }

    public class Landkreis
    {
        [JsonProperty(PropertyName = "stadt")]
        public string stadt { get; set; }
        
        [JsonProperty(PropertyName = "landkreis")]
        public string landkreis { get; set; }

        [JsonProperty(PropertyName = "fallzahlGesamt")]
        public int fallzahlGesamt { get; set; }

        [JsonProperty(PropertyName = "faelle7Lk")]
        public float faelle7Lk { get; set; }

        [JsonProperty(PropertyName = "faellePro100000Ew")]
        public float faellePro100000Ew { get; set; }

        [JsonProperty(PropertyName = "inzidenz7Tage")]
        public float inzidenz7Tage { get; set; }

        [JsonProperty(PropertyName = "todesfaelle")]
        public int todesfaelle { get; set; }

        [JsonProperty(PropertyName = "todesfaelle7Lk")]
        public int todesfaelle7Lk { get; set; }
    }

    public class BlAttribute
    {
        [JsonProperty(PropertyName = "bundesland")]
        public string bundesland { get; set; }

        [JsonProperty(PropertyName = "fallzahlGesamt")]
        public int fallzahlGesamt { get; set; }
     
        [JsonProperty(PropertyName = "faelle7BL")]
        public float faelle7BL { get; set; }

        [JsonProperty(PropertyName = "faellePro100000Ew")]
        public float faellePro100000Ew { get; set; }

        [JsonProperty(PropertyName = "todesfaelle")]
        public int todesfaelle { get; set; }

        [JsonProperty(PropertyName = "todesfaelle7BL")]
        public int todesfaelle7BL { get; set; }

        [JsonProperty(PropertyName = "inzidenz7Tage")]
        public string inzidenz7Tage { get; set; } 
        
        [JsonProperty(PropertyName = "farbe")]
        public string farbe { get; set; }
    }
}
