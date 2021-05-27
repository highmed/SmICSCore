using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.PatientInformation.PatientMovement.ReceiveModels
{
    public class EpisodeOfCareModel
    {
        public DateTime Beginn { get; set; }
        public DateTime? Ende { get; set; }
    }
}
