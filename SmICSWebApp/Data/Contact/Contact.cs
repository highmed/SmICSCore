using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmICSCoreLib.AQL.General;


namespace SmICSWebApp.Data.Contact
{
    public class Contact : Case
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public string Ward { get; set; }
        public bool HasFinding { get; set; }
        public bool HAsFindingAtContactTime { get; set; }
    }
}
