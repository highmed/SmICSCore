using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Cases.ReceiveModel
{
    public class CaseDataReceiveModel
    {
        [JsonProperty(PropertyName = "Anzahl_Faelle")]
        public int Anzahl_Faelle { get; set; }

    }
}
