using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.General
{
    public class PatientLocation
    {
        public string Ward { get; set; }
        public string Departement { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PatientLocation)
            {
                PatientLocation tmp = obj as PatientLocation;
                if (Departement == tmp.Departement)
                {
                    if (Ward == tmp.Ward)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
