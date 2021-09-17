using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.ContactNetwork.ReceiveModels
{
    public class ContactPatientModel
    {
        public string PatientID { get; set; }
        public DateTime Beginn { get; set; }
        public Nullable<DateTime> Ende { get; set; }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == typeof(ContactPatientModel))
            {
                ContactPatientModel tmp = obj as ContactPatientModel;
                if(tmp.PatientID == PatientID && tmp.Beginn == Beginn && tmp.Ende == Ende)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
