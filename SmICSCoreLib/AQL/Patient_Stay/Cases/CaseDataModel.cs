using Newtonsoft.Json;
using SmICSCoreLib.AQL.Patient_Stay.Cases.ReceiveModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Cases
{
    public class CaseDataModel
    {
        [JsonProperty(PropertyName = "Anzahl_Faelle")]
        public int Anzahl_Faelle { get; set; }

        public CaseDataModel() { }
        public CaseDataModel(CaseDataReceiveModel caseDataReceiveModel)
        {
            Anzahl_Faelle = caseDataReceiveModel.Anzahl_Faelle;
        }
    }
}
