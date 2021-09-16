using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSWebApp.Data.PatientView.Models
{
    public class PatientViewData : SortedDictionary<Case, LabDataCollection>
    {
        public LabDataCollection this[string ID]
        {
            get
            {
                Case c = this.Keys.Where(c => c.ID == ID).FirstOrDefault();
                if(c != null)
                {
                    return this[c];
                }
                throw new InvalidOperationException($"No Case with ID: {ID} given!");
            }
        }
    }
}
