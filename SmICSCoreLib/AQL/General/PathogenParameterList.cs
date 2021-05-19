using System;
using System.Collections.Generic;


namespace SmICSCoreLib.AQL.General
{
    public class PathogenParameterList
    {
        public List<string> pathogenList { get; set; }

        public string ToAQLMatchString()
        {
            string convertedPatientList = String.Join("','", pathogenList);
            return "{'" + convertedPatientList + "'}";
        }
    }
}
