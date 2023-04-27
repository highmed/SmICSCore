using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Models.General;
using System;
 
namespace SmICSCoreLib.Factories.ContactNew
{
    public class Contact : Case
    {
        public DateOnly ContactStart { get; set; }
        public DateOnly? ContactEnd { get; set; }

        public Ward? Ward { get; set; }

        public Departement? Departement { get; set; }

        public string? Room { get; set; }
    }
}
