using System;
using System.Collections.Generic;
using System.Text;

//@ TODO Pascal: Für PatientListParameter & EmployeeListParameter eine abstract class zum vererben erstellen oder refactoren zu EHRListParameter
namespace SmICSCoreLib.Factories.General
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
