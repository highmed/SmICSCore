using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NUMNode
{
    public interface INUMNodeFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        public List<NUMNodeModel> Process(TimespanParameter timespan);
        public void FirstDataEntry();
        public void RegularDataEntry();
    }
}
