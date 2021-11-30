using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NEC
{
    public interface INECCombinedFactory
    {
        List<NECPatientInformation> Process(DateTime date);
    }
}