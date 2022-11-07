using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class WardParameter : Case
    {
        public string Ward { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> PathogenCode { get; set; }
        public string DepartementID { get; set; }
    }
}