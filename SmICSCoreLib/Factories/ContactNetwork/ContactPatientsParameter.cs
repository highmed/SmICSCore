using SmICSCoreLib.Factories.ContactNetwork.ReceiveModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.ContactNetwork
{
    public class ContactPatientsParameter : ContactParameter
    {
        public DateTime SuperStarttime { get; set; }
        public DateTime SuperEndtime { get; set; }
        public string WardID { get; set; }
        public string Departement { get; set; }

        public ContactPatientsParameter() { }
        public ContactPatientsParameter(ContactParameter contactParameter, PatientWardModel patientWard)
        {
            PatientID = contactParameter.PatientID;
            Starttime = patientWard.Beginn;
            Endtime = patientWard.Ende;
            SuperStarttime = contactParameter.Starttime;
            SuperEndtime = contactParameter.Endtime;
            Departement = patientWard.Fachabteilung;
            WardID = patientWard.StationID;

        }
    }
}
