using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using System;
using System.Collections.Generic;

namespace SmICSWebApp.Data.Contact
{
    public class Contact : Case
    {
        public InfectionStatus InfectionStatus { get; set; }
        public SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay PatientLocation { get; internal set; }
        public DateTime ContactStart { get; set; }
        public DateTime? ContactEnd { get; set; }
        public bool RoomContact { get; internal set; }
        public bool WardContact { get; internal set; }
        public bool DepartementContact { get; internal set; }
        public DateTime? StatusDate { get; internal set; }
    }
}
