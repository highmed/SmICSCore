using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.RKIStatistics
{
    public interface IRKIReportFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        public void Process(string path);
    }
}
