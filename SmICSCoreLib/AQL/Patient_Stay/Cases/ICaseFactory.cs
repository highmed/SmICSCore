using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Cases
{
    public interface ICaseFactory
    {
        List<CaseDataModel> Process( DateTime date);
    }
}
