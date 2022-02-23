using System.Collections.Generic;

namespace SmICSWebApp.Data.ContactComparison
{
    public class ComparedContact
    {
        public string PatientA { get; internal set; }
        public string PatientB { get; internal set; }
        public List<ContactPoint> ContactLocations { get; internal set; }
    }
}