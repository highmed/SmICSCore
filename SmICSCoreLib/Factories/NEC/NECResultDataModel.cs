using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECResultDataModel
    {
        public string PatientID { get; set; }
        public List<string> StationID { get; set; }
        public List<string> RoomID { get; set; }
        public List<int> MovementTypeID { get; set; }
        public DateTime Date { get; set; }
        public decimal Prediciton { get; set; }
        public bool LabGroundTruth { get; set; }
        public List<string> PathogenGroundTruth { get; set; }
    }
}