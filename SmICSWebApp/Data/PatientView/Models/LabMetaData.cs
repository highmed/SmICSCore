using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSWebApp.Data.PatientView.Models
{
    public class LabMetaData
    {
        public DateTime ReportDate { get; set; }
        public bool Collapsed { get; set; } = false;
    }
}
