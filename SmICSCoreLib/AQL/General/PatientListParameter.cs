using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.General
{
    public class PatientListParameter
    {
        public List<string> patientList { get; set; }

        public string ToAQLMatchString()
        {
            string convertedPatientList = String.Join("','", patientList);
            return "{'" + convertedPatientList + "'}";
        }
    }
}
