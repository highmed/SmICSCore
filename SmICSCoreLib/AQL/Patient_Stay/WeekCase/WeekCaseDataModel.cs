using Newtonsoft.Json;
using SmICSCoreLib.AQL.Patient_Stay.WeekCase.ReceiveModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.WeekCase
{
    public class WeekCaseDataModel
    {
        [JsonProperty(PropertyName = "Anzahl_Faelle")]
        public int Anzahl_Faelle { get; set; }

        public WeekCaseDataModel() { }
        public WeekCaseDataModel(WeekCaseReceiveModel weekCaseReceive ) {

            Anzahl_Faelle = weekCaseReceive.Anzahl_Faelle;
        }
            
    }
}
